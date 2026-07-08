using Microsoft.Playwright;

namespace AutomationExercise.Tests.Pages;

public class OrderConfirmationPage : BasePage
{
    public OrderConfirmationPage(IPage page) : base(page)
    {
    }

    public async Task<bool> IsOrderPlacedAsync()
        => await Page.GetByText("Order Placed!").IsVisibleAsync();

    public async Task ContinueAsync()
    {
        await Page.GetByText("Continue").ClickAsync();
        await DismissCookieConsentIfPresentAsync();
    }
}
