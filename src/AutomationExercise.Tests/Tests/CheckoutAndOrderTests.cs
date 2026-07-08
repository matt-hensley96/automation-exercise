using AutomationExercise.Tests.ApiClients;
using AutomationExercise.Tests.Fixtures;
using AutomationExercise.Tests.Helpers;
using AutomationExercise.Tests.Pages;
using FluentAssertions;

namespace AutomationExercise.Tests.Tests;

public class CheckoutAndOrderTests : PageTestBase
{
    public CheckoutAndOrderTests(TestRunFixture fixture) : base(fixture)
    {
    }

    // The full realistic user journey as a single, independent test: register -> search &
    // open a product -> add to cart with a specific quantity -> checkout -> pay -> confirm
    // order -> logout. Deliberately not split across multiple [Fact]s, since xUnit does not
    // guarantee method execution order within a class and this journey is inherently
    // sequential - splitting it would mean relying on undefined ordering to pass.
    [Fact]
    public async Task PurchaseJourney_RegisterSearchAddToCartCheckoutAndPay_CompletesOrder()
        => await RunStepAsync(async () =>
        {
            var user = TestUserFactory.CreateUniqueUser();
            using var apiClient = new AutomationExerciseApiClient(Settings.ApiBaseUrl);

            try
            {
                // 1. Register a new account through the UI.
                var loginPage = new LoginPage(Page);
                await loginPage.GotoAsync();
                await loginPage.StartSignupAsync(user.Name, user.Email);

                var accountInfoPage = new AccountInformationPage(Page);
                await accountInfoPage.FillAccountInformationAsync(user);
                await accountInfoPage.SubmitAsync();

                var accountCreatedPage = new AccountCreatedPage(Page);
                (await accountCreatedPage.IsAccountCreatedAsync()).Should().BeTrue();
                await accountCreatedPage.ContinueAsync();

                (await accountCreatedPage.IsLoggedInAsAsync(user.Name)).Should().BeTrue();

                // 2. Search for a product and open its details page.
                var productsPage = new ProductsPage(Page);
                await productsPage.GotoAsync();
                await productsPage.SearchAsync("Dress");
                (await productsPage.GetProductCountAsync()).Should().BeGreaterThan(0);
                await productsPage.OpenProductDetailsByIndexAsync(0);

                // 3. Add it to the cart with a specific quantity and verify the subtotal.
                var productDetailsPage = new ProductDetailsPage(Page);
                var productName = await productDetailsPage.GetProductNameAsync();
                var unitPrice = ExtractRupees(await productDetailsPage.GetProductPriceAsync());
                var productId = productDetailsPage.GetProductIdFromUrl();

                await productDetailsPage.SetQuantityAsync(3);
                await productDetailsPage.AddToCartAsync();
                await productDetailsPage.ViewCartFromModalAsync();

                var cartPage = new CartPage(Page);
                (await cartPage.GetQuantityForProductAsync(productId)).Should().Be("3");
                (await cartPage.GetTotalPriceForProductAsync(productId)).Should().Be($"Rs. {unitPrice * 3}");

                // 4. Proceed to checkout - now logged in, so no guest-checkout gate.
                await cartPage.ProceedToCheckoutAsync();

                var checkoutPage = new CheckoutPage(Page);
                var deliveryAddress = await checkoutPage.GetDeliveryAddressAsync();
                deliveryAddress.Should().Contain(user.FirstName).And.Contain(user.Address1);
                (await checkoutPage.GetCartItemCountAsync()).Should().Be(1);
                await checkoutPage.EnterOrderCommentAsync($"Order for {productName} - please deliver during business hours.");
                await checkoutPage.PlaceOrderAsync();

                // 5. Pay with fake card details and confirm the order.
                var paymentPage = new PaymentPage(Page);
                await paymentPage.FillPaymentDetailsAsync(
                    nameOnCard: $"{user.FirstName} {user.LastName}",
                    cardNumber: "4111111111111111",
                    cvc: "123",
                    expiryMonth: "12",
                    expiryYear: "2030");
                await paymentPage.PayAndConfirmOrderAsync();

                var orderConfirmationPage = new OrderConfirmationPage(Page);
                (await orderConfirmationPage.IsOrderPlacedAsync()).Should().BeTrue();
                await orderConfirmationPage.ContinueAsync();

                // 6. Logout and confirm the session is cleared.
                await orderConfirmationPage.LogoutAsync();
                new LoginPage(Page).IsOnLoginPage().Should().BeTrue();
            }
            finally
            {
                // Combined UI+API pattern: the account was created through the UI (what we're
                // testing); cleanup goes through the API so repeated runs never collide.
                await apiClient.DeleteAccountAsync(user.Email, user.Password);
            }
        });

    private static int ExtractRupees(string priceText)
        => int.Parse(new string(priceText.Where(char.IsDigit).ToArray()));
}
