using System.Runtime.CompilerServices;
using AutomationExercise.Tests.Config;
using AutomationExercise.Tests.Helpers;
using AventStack.ExtentReports;

namespace AutomationExercise.Tests.Fixtures;

// API tests don't need a browser/page at all, so this is a lighter sibling of PageTestBase -
// same "Suite collection" (for the shared single-flush ExtentReportManager timing) and the
// same RunStepAsync-wrapper-for-reporting pattern, just without any Playwright setup.
[Collection("Suite collection")]
public abstract class ApiTestBase
{
    private readonly TestRunFixture _fixture;
    protected TestSettings Settings => _fixture.Settings;

    protected ApiTestBase(TestRunFixture fixture)
    {
        _fixture = fixture;
    }

    protected async Task RunStepAsync(Func<Task> testBody, [CallerMemberName] string testName = "")
    {
        ExtentTest extentTest = ExtentReportManager.CreateTest($"{GetType().Name}.{testName}");
        try
        {
            await testBody();
            extentTest.Log(Status.Pass, "Test completed successfully.");
        }
        catch (Exception ex)
        {
            extentTest.Fail(ex.Message);
            await GitHubIssueReporter.ReportFailureAsync($"{GetType().Name}.{testName}", ex.ToString());
            throw;
        }
    }
}
