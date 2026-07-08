using AutomationExercise.Tests.ApiClients;

namespace AutomationExercise.Tests.Helpers;

// Deletes an account created through the UI signup flow, via the REST API, so repeated test
// runs never collide on "email already exists" - a combined UI+API pattern: create through the
// UI (what we're actually testing), clean up through the API (fast, reliable, no extra browser steps).
public static class ApiCleanupHelper
{
    public static async Task DeleteAccountAsync(string apiBaseUrl, string email, string password)
    {
        using var client = new AutomationExerciseApiClient(apiBaseUrl);
        await client.DeleteAccountAsync(email, password);
    }
}
