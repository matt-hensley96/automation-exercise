namespace AutomationExercise.Tests.Helpers;

// automationexercise.com's API always answers with HTTP 200 at the transport level - the real
// status (200/201/400/404/405) is embedded in a "responseCode" field in the JSON body instead.
// Confirmed against the live API: e.g. POST /api/productsList returns transport 200 with body
// {"responseCode": 405, "message": "This request method is not supported."}. ApiResult
// translates that quirk into the field tests actually assert on.
public class ApiResult
{
    public int ResponseCode { get; set; }
    public string Message { get; set; } = "";
    public string RawBody { get; set; } = "";
}
