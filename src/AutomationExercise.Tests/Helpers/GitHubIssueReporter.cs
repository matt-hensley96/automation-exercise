using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AutomationExercise.Tests.Helpers;

public static class GitHubIssueReporter
{
    private static readonly HttpClient HttpClient = CreateHttpClient();

    public static async Task ReportFailureAsync(string testName, string errorDetails, string? screenshotPath = null)
    {
        var gitHubToken = Environment.GetEnvironmentVariable("GH_ISSUE_TOKEN");

        if (string.IsNullOrEmpty(gitHubToken))
            return;

        try
        {
            var runUrl = BuildWorkflowRunUrl();
            
            var body = $"**{testName}** failed:\n\n```\n{errorDetails}\n```" +
                (screenshotPath is null
                    ? ""
                    : $"\n\nScreenshot: `{screenshotPath}`, in the `test-results` artifact of [this workflow run]({runUrl}).");

            var payload = new
            {
                title = $"Automated test failure: {testName}",
                body,
                labels = new[] { "test-failure" }
            };

            var url = $"https://api.github.com/repos/matt-hensley96/automation-exercise/issues";

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", gitHubToken);

            await HttpClient.SendAsync(request);
        }
        catch
        {
            // A ticketing failure shouldn't fail the test run or hide the real assertion failure.
        }
    }

    private static string BuildWorkflowRunUrl()
    {
        var serverUrl = Environment.GetEnvironmentVariable("GITHUB_SERVER_URL");
        var repository = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");
        var runId = Environment.GetEnvironmentVariable("GITHUB_RUN_ID");
        
        return $"{serverUrl}/{repository}/actions/runs/{runId}";
    }

    private static HttpClient CreateHttpClient()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AutomationExercise.Tests");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

        return httpClient;
    }
}
