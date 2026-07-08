using Microsoft.Extensions.Configuration;

namespace AutomationExercise.Tests.Config;

public class TestSettings
{
    public string BaseUrl { get; set; } = "";
    public string ApiBaseUrl { get; set; } = "";
    public bool Headless { get; set; } = true;
    public int SlowMo { get; set; }
    public int TimeoutMs { get; set; } = 30000;

    // Env var override uses the double-underscore convention, e.g. TESTSETTINGS__Headless=false,
    // since colon-separated keys don't work reliably across shells.
    public static TestSettings Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Config/appsettings.json", optional: false)
            .AddEnvironmentVariables(prefix: "TESTSETTINGS__")
            .Build();

        var settings = new TestSettings();
        configuration.GetSection("TestSettings").Bind(settings);
        return settings;
    }
}
