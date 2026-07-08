using Microsoft.Playwright;

namespace AutomationExercise.Tests.Pages;

public class CheckoutPage : BasePage
{
    public CheckoutPage(IPage page) : base(page)
    {
    }

    public async Task<string> GetDeliveryAddressAsync()
        => await Page.Locator("#address_delivery").TextContentAsync() ?? "";

    public async Task<string> GetInvoiceAddressAsync()
        => await Page.Locator("#address_invoice").TextContentAsync() ?? "";

    // #cart_info's tbody has one <tr id="product-N"> per line item plus a trailing, id-less
    // "Total Amount" summary row - confirmed live, so we scope to the id-bearing rows only.
    public async Task<int> GetCartItemCountAsync()
        => await Page.Locator("#cart_info tbody tr[id^='product-']").CountAsync();

    public async Task EnterOrderCommentAsync(string message)
        => await Page.Locator("textarea[name='message']").FillAsync(message);

    public async Task PlaceOrderAsync()
    {
        await Page.GetByText("Place Order").ClickAsync();
        await DismissCookieConsentIfPresentAsync();
    }
}
