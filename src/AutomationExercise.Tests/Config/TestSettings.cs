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
    // since colon-separated keys don't work reliably across shells. No prefix is passed to
    // AddEnvironmentVariables: a prefix gets stripped off before "__" is translated into the
    // config hierarchy separator, which would turn TESTSETTINGS__Headless into a flat top-level
    // "Headless" key instead of "TestSettings:Headless" and silently break the override.
    public static TestSettings Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Config/appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var settings = new TestSettings();
        configuration.GetSection("TestSettings").Bind(settings);
        return settings;
    }
}
