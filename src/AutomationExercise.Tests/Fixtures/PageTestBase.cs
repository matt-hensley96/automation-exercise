using System.Runtime.CompilerServices;
using AutomationExercise.Tests.Config;
using AutomationExercise.Tests.Helpers;
using AventStack.ExtentReports;
using Microsoft.Playwright;

namespace AutomationExercise.Tests.Fixtures;

// Base class for every UI test class. xUnit v2 has no built-in pass/fail hook inside
// IAsyncLifetime.DisposeAsync, so failure detection is done explicitly: test methods call
// RunStepAsync(...) instead of putting arrange/act/assert directly in the [Fact] body. This
// keeps all try/catch/screenshot/reporting plumbing out of the Tests/ classes - they only
// ever contain the call into Page Objects and the assertions.
[Collection("Suite collection")]
public abstract class PageTestBase : IAsyncLifetime
{
    private readonly TestRunFixture _fixture;

    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;
    protected TestSettings Settings => _fixture.Settings;

    protected PageTestBase(TestRunFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        Context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = Settings.BaseUrl,
            ViewportSize = new ViewportSize { Width = 1440, Height = 900 }
        });
        Context.SetDefaultTimeout(Settings.TimeoutMs);
        Page = await Context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.CloseAsync();
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
            var screenshotPath = await CaptureFailureScreenshotAsync(testName);
            extentTest.Fail(ex.Message);
            if (screenshotPath is not null)
            {
                extentTest.AddScreenCaptureFromPath(screenshotPath);
            }
            throw; // re-throw so xUnit's own failure reporting (stack trace, assertion message) is untouched
        }
    }

    private async Task<string?> CaptureFailureScreenshotAsync(string testName)
    {
        try
        {
            var fileName = $"{GetType().Name}.{testName}.{DateTime.UtcNow:yyyyMMddHHmmss}.png";
            var path = Path.Combine(AppContext.BaseDirectory, "TestResults", "Screenshots", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = path, FullPage = true });
            return path;
        }
        catch
        {
            // Screenshot capture is best-effort - a page already torn down by the failure
            // shouldn't mask the original test exception with a screenshot exception.
            return null;
        }
    }
}
