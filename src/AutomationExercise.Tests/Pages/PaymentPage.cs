using Microsoft.Playwright;

namespace AutomationExercise.Tests.Pages;

public class PaymentPage : BasePage
{
    public PaymentPage(IPage page) : base(page)
    {
    }

    public async Task FillPaymentDetailsAsync(string nameOnCard, string cardNumber, string cvc, string expiryMonth, string expiryYear)
    {
        await Page.Locator("[data-qa='name-on-card']").FillAsync(nameOnCard);
        await Page.Locator("[data-qa='card-number']").FillAsync(cardNumber);
        await Page.Locator("[data-qa='cvc']").FillAsync(cvc);
        await Page.Locator("[data-qa='expiry-month']").FillAsync(expiryMonth);
        await Page.Locator("[data-qa='expiry-year']").FillAsync(expiryYear);
    }

    public async Task PayAndConfirmOrderAsync()
    {
        await Page.Locator("[data-qa='pay-button']").ClickAsync();
        await DismissCookieConsentIfPresentAsync();
    }
}
