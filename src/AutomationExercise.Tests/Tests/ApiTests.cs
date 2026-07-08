using System.Text.Json;
using AutomationExercise.Tests.ApiClients;
using AutomationExercise.Tests.Fixtures;
using AutomationExercise.Tests.Helpers;
using FluentAssertions;

namespace AutomationExercise.Tests.Tests;

// Pure REST API coverage against automationexercise.com/api_list
public class ApiTests : ApiTestBase
{
    public ApiTests(TestRunFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetProductsList_ReturnsNonEmptyProductCatalog()
        => await RunStepAsync(async () =>
        {
            using var apiClient = new AutomationExerciseApiClient(Settings.ApiBaseUrl);
            var result = await apiClient.GetProductsListAsync();

            result.ResponseCode.Should().Be(200);
            using var json = JsonDocument.Parse(result.RawBody);
            json.RootElement.GetProperty("products").GetArrayLength().Should().BeGreaterThan(0);
        });

    [Fact]
    public async Task PostProductsList_MethodNotSupported_Returns405()
        => await RunStepAsync(async () =>
        {
            using var apiClient = new AutomationExerciseApiClient(Settings.ApiBaseUrl);
            var result = await apiClient.PostProductsListAsync();

            result.ResponseCode.Should().Be(405);
            result.Message.Should().Contain("not supported");
        });

    [Fact]
    public async Task GetBrandsList_ReturnsNonEmptyBrandList_PutIsRejected()
        => await RunStepAsync(async () =>
        {
            using var apiClient = new AutomationExerciseApiClient(Settings.ApiBaseUrl);

            var getResult = await apiClient.GetBrandsListAsync();
            getResult.ResponseCode.Should().Be(200);
            using (var json = JsonDocument.Parse(getResult.RawBody))
            {
                json.RootElement.GetProperty("brands").GetArrayLength().Should().BeGreaterThan(0);
            }

            var putResult = await apiClient.PutBrandsListAsync();
            putResult.ResponseCode.Should().Be(405);
        });

    [Fact]
    public async Task SearchProduct_WithTerm_ReturnsMatches_WithoutTerm_Returns400()
        => await RunStepAsync(async () =>
        {
            using var apiClient = new AutomationExerciseApiClient(Settings.ApiBaseUrl);

            var withTerm = await apiClient.SearchProductAsync("top");
            withTerm.ResponseCode.Should().Be(200);
            using (var json = JsonDocument.Parse(withTerm.RawBody))
            {
                json.RootElement.GetProperty("products").GetArrayLength().Should().BeGreaterThan(0);
            }

            var withoutTerm = await apiClient.SearchProductAsync(null);
            withoutTerm.ResponseCode.Should().Be(400);
            withoutTerm.Message.Should().Contain("search_product parameter is missing");
        });

    [Fact]
    public async Task VerifyLogin_ValidMissingAndWrongCredentials_ReturnExpectedCodes_DeleteIsRejected()
        => await RunStepAsync(async () =>
        {
            var user = TestUserFactory.CreateUniqueUser();
            using var apiClient = new AutomationExerciseApiClient(Settings.ApiBaseUrl);
            var createResult = await apiClient.CreateAccountAsync(user);
            createResult.ResponseCode.Should().Be(201);

            try
            {
                var validLogin = await apiClient.VerifyLoginAsync(user.Email, user.Password);
                validLogin.ResponseCode.Should().Be(200);
                validLogin.Message.Should().Contain("User exists");

                var missingPassword = await apiClient.VerifyLoginAsync(user.Email, null);
                missingPassword.ResponseCode.Should().Be(400);

                var wrongCredentials = await apiClient.VerifyLoginAsync(user.Email, "TotallyWrongPassword!");
                wrongCredentials.ResponseCode.Should().Be(404);
                wrongCredentials.Message.Should().Contain("not found");

                var deleteVerb = await apiClient.DeleteVerifyLoginAsync();
                deleteVerb.ResponseCode.Should().Be(405);
            }
            finally
            {
                await apiClient.DeleteAccountAsync(user.Email, user.Password);
            }
        });

    [Fact]
    public async Task GetCreateAccount_MethodNotSupported_ReturnsTransport405()
        => await RunStepAsync(async () =>
        {
            using var apiClient = new AutomationExerciseApiClient(Settings.ApiBaseUrl);
            var result = await apiClient.GetCreateAccountAsync();

            // Unlike every other endpoint above, this 405 comes from the real HTTP transport
            // status, not a "responseCode" field in the JSON body - there is no such field here.
            result.ResponseCode.Should().Be(405);
        });

    [Fact]
    public async Task GetDeleteAccount_MethodNotSupported_ReturnsTransport405()
        => await RunStepAsync(async () =>
        {
            using var apiClient = new AutomationExerciseApiClient(Settings.ApiBaseUrl);
            var result = await apiClient.GetDeleteAccountAsync();

            result.ResponseCode.Should().Be(405);
        });

    [Fact]
    public async Task CreateGetAndDeleteAccount_FullLifecycle_Succeeds()
        => await RunStepAsync(async () =>
        {
            var user = TestUserFactory.CreateUniqueUser();
            using var apiClient = new AutomationExerciseApiClient(Settings.ApiBaseUrl);

            var createResult = await apiClient.CreateAccountAsync(user);
            createResult.ResponseCode.Should().Be(201);

            var getResult = await apiClient.GetUserDetailByEmailAsync(user.Email);
            getResult.ResponseCode.Should().Be(200);
            using (var json = JsonDocument.Parse(getResult.RawBody))
            {
                json.RootElement.GetProperty("user").GetProperty("email").GetString().Should().Be(user.Email);
            }

            var deleteResult = await apiClient.DeleteAccountAsync(user.Email, user.Password);
            deleteResult.ResponseCode.Should().Be(200);
            deleteResult.Message.Should().Contain("Account deleted");
        });
}
