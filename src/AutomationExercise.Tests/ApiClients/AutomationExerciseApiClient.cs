using System.Text.Json;
using AutomationExercise.Tests.Helpers;

namespace AutomationExercise.Tests.ApiClients;

// Thin wrapper over automationexercise.com's REST API. All POST/PUT/DELETE bodies are
// form-urlencoded (not JSON) - confirmed against the live API. The API also always answers
// with transport HTTP 200, embedding the real status in a "responseCode" JSON field (see
// ApiResult), so every call here parses the body rather than trusting HttpResponseMessage.StatusCode.
public class AutomationExerciseApiClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public AutomationExerciseApiClient(string apiBaseUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
    }

    public Task<ApiResult> GetProductsListAsync() => SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/productsList"));

    public Task<ApiResult> PostProductsListAsync() => SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/productsList"));

    public Task<ApiResult> GetBrandsListAsync() => SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/brandsList"));

    public Task<ApiResult> PutBrandsListAsync() => SendAsync(new HttpRequestMessage(HttpMethod.Put, "/api/brandsList"));

    public Task<ApiResult> SearchProductAsync(string? searchTerm)
    {
        var fields = new Dictionary<string, string>();
        if (searchTerm is not null)
        {
            fields["search_product"] = searchTerm;
        }
        return SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/searchProduct") { Content = new FormUrlEncodedContent(fields) });
    }

    public Task<ApiResult> VerifyLoginAsync(string? email, string? password)
    {
        var fields = new Dictionary<string, string>();
        if (email is not null) fields["email"] = email;
        if (password is not null) fields["password"] = password;
        return SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/verifyLogin") { Content = new FormUrlEncodedContent(fields) });
    }

    public Task<ApiResult> DeleteVerifyLoginAsync() => SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/verifyLogin"));

    public Task<ApiResult> CreateAccountAsync(AccountInfo account)
        => SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/createAccount") { Content = ToFormContent(account) });

    public Task<ApiResult> DeleteAccountAsync(string email, string password)
        => SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/deleteAccount")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string> { ["email"] = email, ["password"] = password })
        });

    public Task<ApiResult> GetUserDetailByEmailAsync(string email)
        => SendAsync(new HttpRequestMessage(HttpMethod.Get, $"/api/getUserDetailByEmail?email={Uri.EscapeDataString(email)}"));

    private static FormUrlEncodedContent ToFormContent(AccountInfo account) => new(new Dictionary<string, string>
    {
        ["name"] = account.Name,
        ["email"] = account.Email,
        ["password"] = account.Password,
        ["title"] = account.Title,
        ["birth_date"] = account.BirthDay,
        ["birth_month"] = account.BirthMonth,
        ["birth_year"] = account.BirthYear,
        ["firstname"] = account.FirstName,
        ["lastname"] = account.LastName,
        ["company"] = account.Company,
        ["address1"] = account.Address1,
        ["address2"] = account.Address2,
        ["country"] = account.Country,
        ["zipcode"] = account.ZipCode,
        ["state"] = account.State,
        ["city"] = account.City,
        ["mobile_number"] = account.MobileNumber
    });

    private async Task<ApiResult> SendAsync(HttpRequestMessage request)
    {
        var response = await _httpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        using var json = JsonDocument.Parse(body);
        var root = json.RootElement;
        return new ApiResult
        {
            ResponseCode = root.TryGetProperty("responseCode", out var code) ? code.GetInt32() : (int)response.StatusCode,
            Message = root.TryGetProperty("message", out var message) ? message.GetString() ?? "" : "",
            RawBody = body
        };
    }

    public void Dispose() => _httpClient.Dispose();
}
