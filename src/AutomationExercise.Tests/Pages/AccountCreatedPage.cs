using Microsoft.Playwright;

namespace AutomationExercise.Tests.Pages;

public class AccountCreatedPage : BasePage
{
    public AccountCreatedPage(IPage page) : base(page)
    {
    }

    public async Task<bool> IsAccountCreatedAsync()
        => await Page.Locator("[data-qa='account-created']").IsVisibleAsync();

    public async Task ContinueAsync()
    {
        await Page.Locator("[data-qa='continue-button']").ClickAsync();
        await DismissCookieConsentIfPresentAsync();
    }
}
