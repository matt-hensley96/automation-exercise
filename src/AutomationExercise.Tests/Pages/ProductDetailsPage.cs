using Microsoft.Playwright;

namespace AutomationExercise.Tests.Pages;

public class ProductDetailsPage : BasePage
{
    public ProductDetailsPage(IPage page) : base(page)
    {
    }

    public async Task GotoAsync(int productId)
    {
        await Page.GotoAsync($"/product_details/{productId}");
        await DismissCookieConsentIfPresentAsync();
    }

    // Used when this page was reached via search + "View Product" rather than a known,
    // hard-coded product id.
    public int GetProductIdFromUrl() => int.Parse(Page.Url.Split("/product_details/")[1].Split('/', '?')[0]);

    public async Task<string> GetProductNameAsync()
        => await Page.Locator(".product-information h2").TextContentAsync() ?? "";

    public async Task<string> GetProductPriceAsync()
        => await Page.Locator(".product-information span span").TextContentAsync() ?? "";

    public async Task SetQuantityAsync(int quantity)
    {
        var input = Page.Locator("#quantity");
        await input.FillAsync(quantity.ToString());
    }

    public async Task AddToCartAsync() => await Page.Locator(".product-information .btn.cart").ClickAsync();

    public async Task ContinueShoppingFromModalAsync()
        => await Page.Locator(".modal-footer .close-modal").ClickAsync();

    public async Task ViewCartFromModalAsync()
    {
        await Page.Locator(".modal-body a[href='/view_cart']").ClickAsync();
        await DismissCookieConsentIfPresentAsync();
    }
}
