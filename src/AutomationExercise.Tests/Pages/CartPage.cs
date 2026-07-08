using Microsoft.Playwright;

namespace AutomationExercise.Tests.Pages;

public class CartPage : BasePage
{
    public CartPage(IPage page) : base(page)
    {
    }

    public async Task GotoAsync()
    {
        await Page.GotoAsync("/view_cart");
        await DismissCookieConsentIfPresentAsync();
    }

    public async Task<string> GetQuantityForProductAsync(int productId)
        => await Page.Locator($"#product-{productId} .cart_quantity button").TextContentAsync() ?? "";

    public async Task<string> GetTotalPriceForProductAsync(int productId)
        => await Page.Locator($"#product-{productId} .cart_total_price").TextContentAsync() ?? "";

    public async Task RemoveProductAsync(int productId)
        => await Page.Locator($"#product-{productId} .cart_quantity_delete").ClickAsync();

    public async Task<bool> IsEmptyAsync() => await Page.Locator("#empty_cart").IsVisibleAsync();

    // Clicking "Proceed To Checkout" while logged in navigates straight to /checkout; while
    // logged out it opens #checkoutModal instead (see IsGuestCheckoutPromptShownAsync).
    public async Task ProceedToCheckoutAsync()
    {
        await Page.Locator(".check_out").ClickAsync();
        await DismissCookieConsentIfPresentAsync();
    }

    public async Task<bool> IsGuestCheckoutPromptShownAsync()
        => await Page.Locator("#checkoutModal").GetByText("Register / Login account to proceed on checkout.").IsVisibleAsync();
}
