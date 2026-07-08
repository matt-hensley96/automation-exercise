using Microsoft.Playwright;

namespace AutomationExercise.Tests.Pages;

public class HomePage : BasePage
{
    public HomePage(IPage page) : base(page)
    {
    }

    public async Task GotoAsync()
    {
        await Page.GotoAsync("/");
        await DismissCookieConsentIfPresentAsync();
    }

    public async Task NavigateToProductsAsync()
    {
        await Page.ClickAsync("a[href='/products']");
        await DismissCookieConsentIfPresentAsync();
    }

    public async Task NavigateToLoginAsync()
    {
        await Page.ClickAsync("a[href='/login']");
        await DismissCookieConsentIfPresentAsync();
    }

    public async Task NavigateToCartAsync()
    {
        await Page.ClickAsync("a[href='/view_cart']");
        await DismissCookieConsentIfPresentAsync();
    }

    // The input id has a typo ("susbscribe") baked into the live site's own markup - not ours.
    public async Task SubscribeAsync(string email)
    {
        var input = Page.Locator("#susbscribe_email");
        await input.ScrollIntoViewIfNeededAsync();
        await input.FillAsync(email);
        await Page.Locator("#subscribe").ClickAsync();
    }

    public async Task<bool> IsSubscriptionSuccessMessageVisibleAsync()
        => await Page.Locator("#success-subscribe").IsVisibleAsync();

    // Used for the invalid-email-format boundary test: rather than guessing whether the site
    // shows a custom error, we ask the browser directly whether the <input type="email">
    // considers the value valid (HTML5 constraint validation).
    public async Task<bool> IsSubscribeEmailFieldValidAsync()
    {
        var input = Page.Locator("#susbscribe_email");
        return await input.EvaluateAsync<bool>("el => el.validity.valid");
    }
}
