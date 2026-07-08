using Microsoft.Playwright;

namespace AutomationExercise.Tests.Pages;

// The /login page hosts two independent forms side by side: "Login to your account" and
// "New User Signup!". Both live on this one Page Object since they share a URL and page state.
public class LoginPage : BasePage
{
    public LoginPage(IPage page) : base(page)
    {
    }

    public async Task GotoAsync()
    {
        await Page.GotoAsync("/login");
        await DismissCookieConsentIfPresentAsync();
    }

    public async Task LoginAsync(string email, string password)
    {
        await Page.Locator("[data-qa='login-email']").FillAsync(email);
        await Page.Locator("[data-qa='login-password']").FillAsync(password);
        await Page.Locator("[data-qa='login-button']").ClickAsync();
        await DismissCookieConsentIfPresentAsync();
    }

    public async Task StartSignupAsync(string name, string email)
    {
        await Page.Locator("[data-qa='signup-name']").FillAsync(name);
        await Page.Locator("[data-qa='signup-email']").FillAsync(email);
        await Page.Locator("[data-qa='signup-button']").ClickAsync();
        await DismissCookieConsentIfPresentAsync();
    }

    public async Task<string> GetLoginErrorMessageAsync()
        => await Page.GetByText("Your email or password is incorrect!").TextContentAsync() ?? "";

    public async Task<string> GetSignupErrorMessageAsync()
        => await Page.GetByText("Email Address already exist!").TextContentAsync() ?? "";

    public bool IsOnLoginPage() => Page.Url.Contains("/login");
}
