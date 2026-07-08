using Bogus;

namespace AutomationExercise.Tests.Helpers;

// Generates a unique fake user per call. Uniqueness matters because the site rejects signup
// with an email that's already registered - without a per-run suffix, a second CI run of the
// same test would collide with the account the previous run created.
public static class TestUserFactory
{
    public static AccountInfo CreateUniqueUser()
    {
        var faker = new Faker();
        var uniqueSuffix = Guid.NewGuid().ToString("N")[..10];

        return new AccountInfo
        {
            Name = faker.Name.FirstName(),
            Email = $"qa.{uniqueSuffix}@mailinator.com",
            Password = "P@ssw0rd123!",
            Title = faker.PickRandom("Mr", "Mrs"),
            BirthDay = "10",
            BirthMonth = "5",
            BirthYear = "1990",
            FirstName = faker.Name.FirstName(),
            LastName = faker.Name.LastName(),
            Company = faker.Company.CompanyName(),
            Address1 = faker.Address.StreetAddress(),
            Address2 = faker.Address.SecondaryAddress(),
            Country = "United States",
            State = faker.Address.State(),
            City = faker.Address.City(),
            ZipCode = faker.Address.ZipCode(),
            MobileNumber = faker.Phone.PhoneNumber("##########")
        };
    }
}
