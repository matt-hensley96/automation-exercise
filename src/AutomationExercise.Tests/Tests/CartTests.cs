using AutomationExercise.Tests.Fixtures;
using AutomationExercise.Tests.Pages;
using FluentAssertions;

namespace AutomationExercise.Tests.Tests;

public class CartTests : PageTestBase
{
    public CartTests(TestRunFixture fixture) : base(fixture)
    {
    }

    // Confirmed against the live site: the quantity input carries a "min=1" HTML attribute,
    // but the server does not enforce it - quantity 0 is silently accepted and added to the
    // cart as a Rs. 0 line item. This test pins that gap as a known, observed behavior.
    [Fact]
    public async Task AddToCart_WithZeroQuantity_IsAcceptedWithZeroTotal()
        => await RunStepAsync(async () =>
        {
            var productDetailsPage = new ProductDetailsPage(Page);
            await productDetailsPage.GotoAsync(1); // Blue Top, Rs. 500
            await productDetailsPage.SetQuantityAsync(0);
            await productDetailsPage.AddToCartAsync();
            await productDetailsPage.ViewCartFromModalAsync();

            var cartPage = new CartPage(Page);
            (await cartPage.GetQuantityForProductAsync(1)).Should().Be("0");
            (await cartPage.GetTotalPriceForProductAsync(1)).Should().Be("Rs. 0");
        });

    // Confirmed against the live site: there is no upper bound either - a very large quantity
    // is accepted and the total is computed correctly (price * quantity, no overflow/rejection).
    [Fact]
    public async Task AddToCart_WithVeryLargeQuantity_ComputesCorrectTotal()
        => await RunStepAsync(async () =>
        {
            var productDetailsPage = new ProductDetailsPage(Page);
            await productDetailsPage.GotoAsync(3); // Sleeveless Dress, Rs. 1000
            await productDetailsPage.SetQuantityAsync(9999);
            await productDetailsPage.AddToCartAsync();
            await productDetailsPage.ViewCartFromModalAsync();

            var cartPage = new CartPage(Page);
            (await cartPage.GetQuantityForProductAsync(3)).Should().Be("9999");
            (await cartPage.GetTotalPriceForProductAsync(3)).Should().Be("Rs. 9999000");
        });

    [Fact]
    public async Task ProceedToCheckout_WhileLoggedOut_ShowsRegisterLoginPrompt()
        => await RunStepAsync(async () =>
        {
            var productDetailsPage = new ProductDetailsPage(Page);
            await productDetailsPage.GotoAsync(1);
            await productDetailsPage.SetQuantityAsync(1);
            await productDetailsPage.AddToCartAsync();
            await productDetailsPage.ViewCartFromModalAsync();

            var cartPage = new CartPage(Page);
            await cartPage.ProceedToCheckoutAsync();

            (await cartPage.IsGuestCheckoutPromptShownAsync()).Should().BeTrue();
            Page.Url.Should().NotContain("/checkout", "a logged-out user must be gated by the Register/Login prompt, not allowed straight through to checkout");
        });

    // Confirmed against the live site: native HTML5 constraint validation on the <input
    // type="email" required> blocks the form's submit event before subscription.js's handler
    // (which unconditionally shows the success message) ever runs.
    [Fact]
    public async Task Subscribe_WithInvalidEmailFormat_IsBlockedByClientSideValidation()
        => await RunStepAsync(async () =>
        {
            var homePage = new HomePage(Page);
            await homePage.GotoAsync();
            await homePage.SubscribeAsync("not-an-email");

            (await homePage.IsSubscribeEmailFieldValidAsync()).Should().BeFalse();
            (await homePage.IsSubscriptionSuccessMessageVisibleAsync()).Should().BeFalse();
        });
}
