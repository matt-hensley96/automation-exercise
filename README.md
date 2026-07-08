# AutomationExercise.com Test Automation Suite

A C# / .NET 8 / Playwright / xUnit test automation framework for
[automationexercise.com](https://automationexercise.com), covering both its UI (e-commerce
journey: signup, browse, cart, checkout, payment) and its public REST API.

## Choice of site to test

Of the suggested sites, I chose to test [automationexercise.com](https://automationexercise.com) due to the well-defined user paths that could be tested and also the fact that its REST API is documented. This meant I could include some API tests in the suite and also use the API as part of the teardown at the end of the tests to delete test data etc.

Other options were:
- [saucedemo.com](https://saucedemo.com) (couldn't see obvious API documentation)
- [restful-booker](https://restful-booker.herokuap.com) (seemed to be more greared towards to API testing than UI)
- [demoqa.com](https://demoqa.com) (probably also a good option, but less natural user journey, more of a demo of web elements)

## Tech stack

| Concern | Choice |
|---|---|
| Language / runtime | C# / .NET 8 |
| Test runner | xUnit |
| UI automation | Playwright for .NET |
| Design pattern | Page Object Model (`Pages/`) - no selectors or interactions in `Tests/` |
| Assertions | FluentAssertions |
| Configuration | `appsettings.json` with option to override environment variables |
| Reporting | ExtentReports (self-contained HTML report) with screenshot on failure |
| CI/CD | GitHub Actions YAML |


## Prerequisites

- **Internet access** - the suite runs against the live public site and its live API, not a local
  mock.
- **.NET 8 SDK** -  If not installed, either download from
  [dotnet.microsoft.com](https://dotnet.microsoft.com/download) or run the following install
  script:
  ```bash
  curl -sSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
  chmod +x dotnet-install.sh
  ./dotnet-install.sh --channel 8.0 --install-dir "$HOME/.dotnet"
  export PATH="$HOME/.dotnet:$PATH"
  export DOTNET_ROOT="$HOME/.dotnet"
  ```
- **PowerShell (`pwsh`)** - This is used to run the `playwright.ps1` browser installer script that Playwright generates in the build output. 
  See
  [Microsoft's install instructions](https://learn.microsoft.com/powershell/scripting/install/installing-powershell).
  
  GitHub Actions' `ubuntu-latest` runner ships
  `pwsh` preinstalled, so no extra step when running the tests on there. 

## Configuration

`src/AutomationExercise.Tests/Config/appsettings.json`:

```json
{
  "TestSettings": {
    "BaseUrl": "https://automationexercise.com",
    "ApiBaseUrl": "https://automationexercise.com",
    "Headless": true,
    "SlowMo": 0,
    "TimeoutMs": 30000
  }
}
```

Every value can be overridden with an environment variable, using the standard
`Microsoft.Extensions.Configuration` double-underscore convention (`TESTSETTINGS__<Key>`).

This allows values to be set when executing the tests via the command line (or CI/CD), e.g.

```bash
TESTSETTINGS__Headless=false dotnet test   # run tests while watching browser instead of running headless
```

## Running the full suite

From the repository root, run the commands below.

This builds the project, installs the Chromium browser Playwright needs, and runs all 16 test scenarios against the live site. 

A full run takes roughly 2 minutes.

```bash
dotnet restore

dotnet build

pwsh src/AutomationExercise.Tests/bin/Debug/net8.0/playwright.ps1 install --with-deps chromium

$env:TESTSETTINGS__Headless = "false"
$env:TESTSETTINGS__SlowMo = "500"

dotnet test
```

## Viewing results

- **HTML report**: `src/AutomationExercise.Tests/bin/Debug/net8.0/TestResults/report.html`
- **Failure screenshots**: `src/AutomationExercise.Tests/bin/Debug/net8.0/TestResults/Screenshots/`,
  one PNG per failed test, named `{TestClass}.{TestName}.{timestamp}.png` so they can be matched
  back to the report by name.

In CI, both are uploaded as a single `test-results` artifact on every run (pass or fail).

## Test coverage (16 scenarios)

- **`Tests/CheckoutAndOrderTests.cs`** - one independent, full end-to-end purchase journey, kept as a single test rather than split across several, since xUnit does not guarantee method execution order within a class and this journey is inherently sequential:
  - register
  - search & open a product
  - add to cart with a specific quantity
  - checkout
  - pay
  - confirm order
  - logout. 
- **`Tests/AccountManagementTests.cs`** - negative cases: login with wrong password, login with
  a nonexistent email, signup with an already-registered email.
- **`Tests/ProductSearchTests.cs`** - error states: empty search term, search term with no
  matches.
- **`Tests/CartTests.cs`** - boundary values (quantity 0, quantity 9999) and negative cases
  (guest checkout gate, invalid subscribe email format).
- **`Tests/ApiTests.cs`** - REST API coverage: products/brands list (success + method-not-allowed),
  search (success + missing-parameter), login verification (success/missing-field/not-found/
  method-not-allowed), and a full create -> read -> delete account lifecycle.

## AI Usage Declaration

**Tool used:** Claude (Claude Code CLI, Anthropic), Sonnet 5 model.

**What it was used for:**
- Generating the initial implementation using the task requirements
- Suggesting fixes for issues that I identified later while running the test suite

**How I validated and improved the solution:**
- Reviewed every generated file and ensured I understood / agreed with the implementation
- Reviewed selectors, assertions and page object structure against the task requirements
- Executed the test suite locally and verified that all tests passed
- Executed the test suite via CI/CD (Github Actions) and then identified/fixed an issue where the screenshots were not displaying property in the HTML test report
- Identified/fixed an issue where a test was failing due to an overlayed ad
- Wrote and added this README file