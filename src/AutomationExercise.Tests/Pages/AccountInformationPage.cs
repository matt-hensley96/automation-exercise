using AutomationExercise.Tests.Helpers;
using Microsoft.Playwright;

namespace AutomationExercise.Tests.Pages;

// The "Enter Account Information" page shown after starting signup on LoginPage. Name and
// email are pre-filled/disabled here (carried over from the signup form), everything else is
// entered fresh.
public class AccountInformationPage : BasePage
{
    public AccountInformationPage(IPage page) : base(page)
    {
    }

    public async Task FillAccountInformationAsync(AccountInfo info)
    {
        await Page.Locator($"#id_gender{(info.Title == "Mr" ? 1 : 2)}").CheckAsync();
        await Page.Locator("[data-qa='password']").FillAsync(info.Password);
        await Page.Locator("[data-qa='days']").SelectOptionAsync(info.BirthDay);
        await Page.Locator("[data-qa='months']").SelectOptionAsync(info.BirthMonth);
        await Page.Locator("[data-qa='years']").SelectOptionAsync(info.BirthYear);

        await Page.Locator("[data-qa='first_name']").FillAsync(info.FirstName);
        await Page.Locator("[data-qa='last_name']").FillAsync(info.LastName);
        await Page.Locator("[data-qa='company']").FillAsync(info.Company);
        await Page.Locator("[data-qa='address']").FillAsync(info.Address1);
        await Page.Locator("[data-qa='address2']").FillAsync(info.Address2);
        await Page.Locator("[data-qa='country']").SelectOptionAsync(info.Country);
        await Page.Locator("[data-qa='state']").FillAsync(info.State);
        await Page.Locator("[data-qa='city']").FillAsync(info.City);
        await Page.Locator("[data-qa='zipcode']").FillAsync(info.ZipCode);
        await Page.Locator("[data-qa='mobile_number']").FillAsync(info.MobileNumber);
    }

    public async Task SubmitAsync()
    {
        await Page.Locator("[data-qa='create-account']").ClickAsync();
        await DismissCookieConsentIfPresentAsync();
    }
}
