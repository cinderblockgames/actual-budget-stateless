namespace ABS.Configuration;

public class EnvironmentVariables
{
    public string ApiUrl { get; }
    public string ApiKey { get; }
    public string BudgetSyncId { get; }
    public string? ActualLinkUrl { get; }

    private EnvironmentVariables(
        string apiUrl, string apiKey, string budgetSyncId,
        string? actualLinkUrl)
    {
        ApiUrl = apiUrl;
        ApiKey = apiKey;
        BudgetSyncId = budgetSyncId;
        ActualLinkUrl = actualLinkUrl;
    }

    public static EnvironmentVariables Build()
    {
        var env = Environment.GetEnvironmentVariables();
        
        // -----------------
        //   ACTUAL
        // -----------------
        
        var apiUrl = env["API_URL"] as string;
        if (string.IsNullOrWhiteSpace(apiUrl))
        {
            throw new Exception("API_URL must be valued.");
        }

        var apiKey = env["API_KEY"] as string;
        var apiKeyFile = env["API_KEY_FILE"] as string;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            if (string.IsNullOrWhiteSpace(apiKeyFile) || !File.Exists(apiKeyFile))
            {
                throw new Exception("API_KEY or API_KEY_FILE (and related file) must be valued.");
            }

            apiKey = File.ReadAllText(apiKeyFile);
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new Exception("API_KEY or API_KEY_FILE (and related file) must be valued.");
            }
        }
        
        var budgetSyncId = env["BUDGET_SYNC_ID"] as string;
        if (string.IsNullOrWhiteSpace(budgetSyncId))
        {
            throw new Exception("BUDGET_SYNC_ID must be valued.");
        }

        var actualLinkUrl = env["ACTUAL_LINK_URL"] as string;

        return new EnvironmentVariables(
            apiUrl, apiKey, budgetSyncId,
            actualLinkUrl);
    }
}