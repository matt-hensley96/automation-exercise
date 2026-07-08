using Microsoft.Playwright;

namespace AutomationExercise.Tests.Pages;

// Shared base for every Page Object. The header/nav (logout link, "Logged in as X") is present
// on every page of the site, so those interactions live here rather than being duplicated.
public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }

    public async Task LogoutAsync()
    {
        await Page.ClickAsync("a[href='/logout']");
        await DismissCookieConsentIfPresentAsync();
    }

    public async Task<bool> IsLoggedInAsAsync(string username)
    {
        return await Page.GetByText($"Logged in as {username}").IsVisibleAsync();
    }

    // The site embeds a Google "Funding Choices" ad-consent overlay (.fc-consent-root) that
    // reappears on every fresh full-page load and blocks pointer events until dismissed - it's
    // injected client-side by an ad script, so it never showed up in a static curl fetch during
    // page research, only once we started driving a real browser. Every Page Object method that
    // causes a full page navigation calls this afterward; a short timeout keeps it cheap on the
    // (common) pages where the overlay doesn't reappear.
    protected async Task DismissCookieConsentIfPresentAsync()
    {
        try
        {
            await Page.Locator(".fc-cta-consent").ClickAsync(new LocatorClickOptions { Timeout = 4000 });
        }
        catch (TimeoutException)
        {
            // No consent overlay appeared - nothing to dismiss.
        }
    }
}
