namespace AutomationExercise.Tests.Helpers;

// Shared data model for both the UI signup flow (AccountInformationPage) and the REST API
// (/api/createAccount, /api/updateAccount) - the two surfaces take the same fields.
public class AccountInfo
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string Title { get; set; } = "Mr"; // "Mr" or "Mrs"
    public string BirthDay { get; set; } = "10";
    public string BirthMonth { get; set; } = "5";
    public string BirthYear { get; set; } = "1990";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Company { get; set; } = "";
    public string Address1 { get; set; } = "";
    public string Address2 { get; set; } = "";
    public string Country { get; set; } = "United States";
    public string State { get; set; } = "";
    public string City { get; set; } = "";
    public string ZipCode { get; set; } = "";
    public string MobileNumber { get; set; } = "";
}
