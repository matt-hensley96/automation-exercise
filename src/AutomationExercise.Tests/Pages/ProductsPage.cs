using Microsoft.Playwright;

namespace AutomationExercise.Tests.Pages;

// Each product card (.product-image-wrapper > .single-products) renders "Add to cart" twice -
// once always-visible inside .productinfo, once inside the hover-only .product-overlay. Both
// share the same data-product-id, so every locator here is scoped to .productinfo specifically
// to avoid a Playwright strict-mode violation (two matches for one selector).
public class ProductsPage : BasePage
{
    public ProductsPage(IPage page) : base(page)
    {
    }

    public async Task GotoAsync()
    {
        await Page.GotoAsync("/products");
        await DismissCookieConsentIfPresentAsync();
    }

    // The search button navigates via window.location.href = '/products?search=...' (a full
    // page reload), so the consent overlay must be dismissed again afterward.
    public async Task SearchAsync(string term)
    {
        await Page.Locator("#search_product").FillAsync(term);
        await Page.Locator("#submit_search").ClickAsync();
        await DismissCookieConsentIfPresentAsync();
    }

    public async Task<int> GetProductCountAsync() => await Page.Locator(".product-image-wrapper").CountAsync();

    public async Task<List<string>> GetProductNamesAsync()
        => (await Page.Locator(".single-products .productinfo p").AllTextContentsAsync()).ToList();

    public async Task AddProductToCartByIndexAsync(int index)
    {
        var card = Page.Locator(".product-image-wrapper").Nth(index);
        await card.Locator(".productinfo a.add-to-cart").ClickAsync();
    }

    public async Task OpenProductDetailsByIndexAsync(int index)
    {
        var card = Page.Locator(".product-image-wrapper").Nth(index);
        await card.Locator(".choose a").ClickAsync();
        await DismissCookieConsentIfPresentAsync();
    }

    public async Task ContinueShoppingFromModalAsync()
        => await Page.Locator(".modal-footer .close-modal").ClickAsync();

    public async Task ViewCartFromModalAsync()
        => await Page.Locator(".modal-body a[href='/view_cart']").ClickAsync();
}
