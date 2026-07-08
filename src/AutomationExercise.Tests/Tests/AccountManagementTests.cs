using AutomationExercise.Tests.ApiClients;
using AutomationExercise.Tests.Fixtures;
using AutomationExercise.Tests.Helpers;
using AutomationExercise.Tests.Pages;
using FluentAssertions;

namespace AutomationExercise.Tests.Tests;

public class AccountManagementTests : PageTestBase
{
    public AccountManagementTests(TestRunFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShowsError()
        => await RunStepAsync(async () =>
        {
            var user = TestUserFactory.CreateUniqueUser();
            using var apiClient = new AutomationExerciseApiClient(Settings.ApiBaseUrl);
            var createResult = await apiClient.CreateAccountAsync(user);
            createResult.ResponseCode.Should().Be(201);

            try
            {
                var loginPage = new LoginPage(Page);
                await loginPage.GotoAsync();
                await loginPage.LoginAsync(user.Email, "DefinitelyWrongPassword123!");

                var errorMessage = await loginPage.GetLoginErrorMessageAsync();
                errorMessage.Should().Contain("incorrect");
                loginPage.IsOnLoginPage().Should().BeTrue();
            }
            finally
            {
                await apiClient.DeleteAccountAsync(user.Email, user.Password);
            }
        });

    [Fact]
    public async Task Login_WithNonexistentEmail_ShowsError()
        => await RunStepAsync(async () =>
        {
            var loginPage = new LoginPage(Page);
            await loginPage.GotoAsync();
            await loginPage.LoginAsync($"qa.nonexistent.{Guid.NewGuid():N}@mailinator.com", "SomePassword123!");

            var errorMessage = await loginPage.GetLoginErrorMessageAsync();
            errorMessage.Should().Contain("incorrect");
            loginPage.IsOnLoginPage().Should().BeTrue();
        });

    [Fact]
    public async Task Signup_WithAlreadyRegisteredEmail_ShowsError()
        => await RunStepAsync(async () =>
        {
            var existingUser = TestUserFactory.CreateUniqueUser();
            using var apiClient = new AutomationExerciseApiClient(Settings.ApiBaseUrl);
            var createResult = await apiClient.CreateAccountAsync(existingUser);
            createResult.ResponseCode.Should().Be(201, "the duplicate-signup path can only be tested once this email is already registered");

            try
            {
                var loginPage = new LoginPage(Page);
                await loginPage.GotoAsync();
                await loginPage.StartSignupAsync("Another Name", existingUser.Email);

                var errorMessage = await loginPage.GetSignupErrorMessageAsync();
                errorMessage.Should().Contain("already exist");
            }
            finally
            {
                await apiClient.DeleteAccountAsync(existingUser.Email, existingUser.Password);
            }
        });
}
