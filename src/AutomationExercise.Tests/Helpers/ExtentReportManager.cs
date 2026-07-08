using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace AutomationExercise.Tests.Helpers;

// Owns a single ExtentReports instance for the whole test run. TestRunFixture (a collection
// fixture shared by every test class) calls Flush() exactly once, in its DisposeAsync, after
// every test in the assembly has finished - see TestRunFixture for why a single shared
// collection is required to make that timing guarantee.
public static class ExtentReportManager
{
    private static readonly Lazy<ExtentReports> ExtentInstance = new(CreateExtentReports);
    private static readonly string ReportDirectory = Path.Combine(AppContext.BaseDirectory, "TestResults");

    public static string ReportPath => Path.Combine(ReportDirectory, "report.html");

    public static ExtentTest CreateTest(string name) => ExtentInstance.Value.CreateTest(name);

    public static void Flush() => ExtentInstance.Value.Flush();

    private static ExtentReports CreateExtentReports()
    {
        Directory.CreateDirectory(ReportDirectory);
        var sparkReporter = new ExtentSparkReporter(ReportPath);
        var extentReports = new ExtentReports();
        extentReports.AttachReporter(sparkReporter);
        return extentReports;
    }
}
