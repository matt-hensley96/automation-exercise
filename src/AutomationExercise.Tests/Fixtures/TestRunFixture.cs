using AutomationExercise.Tests.Config;
using AutomationExercise.Tests.Helpers;
using Microsoft.Playwright;

namespace AutomationExercise.Tests.Fixtures;

// One IPlaywright + one IBrowser for the entire test run (launching a browser is the expensive
// part, so we share it across all tests; per-test isolation still comes from a fresh
// IBrowserContext in PageTestBase). Every test class - UI and API - is attached to the single
// "Suite collection" below, so this fixture's DisposeAsync is guaranteed to run exactly once,
// after every test in the assembly has finished. That is what lets us safely call
// ExtentReportManager.Flush() here: flushing earlier would risk writing the report before
// tests still in flight have logged their result.
public class TestRunFixture : IAsyncLifetime
{
    public TestSettings Settings { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;

    private IPlaywright _playwright = null!;

    public async Task InitializeAsync()
    {
        Settings = TestSettings.Load();
        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Settings.Headless,
            SlowMo = Settings.SlowMo
        });
    }

    public async Task DisposeAsync()
    {
        await Browser.CloseAsync();
        _playwright.Dispose();
        ExtentReportManager.Flush();
    }
}

[CollectionDefinition("Suite collection")]
public class SuiteCollection : ICollectionFixture<TestRunFixture>
{
    // Marker class only - xUnit wires it up via the ICollectionFixture<T> interface.
}
