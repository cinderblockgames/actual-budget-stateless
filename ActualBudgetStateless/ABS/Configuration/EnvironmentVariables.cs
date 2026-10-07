namespace ABS.Configuration;

public class EnvironmentVariables
{
    public string ApiUrl { get; }
    public string ApiKey { get; }
    public string? ActualLinkUrl { get; }
    public Guid[] BankSyncBudgetIds { get; }
    public string? CustomCssPath { get; }
    public string? CustomJavascriptPath { get; }

    private EnvironmentVariables(
        string apiUrl, string apiKey,
        string? actualLinkUrl,
        Guid[] bankSyncBudgetIds,
        string? customCssPath, string? customJavascriptPath)
    {
        ApiUrl = apiUrl;
        ApiKey = apiKey;
        ActualLinkUrl = actualLinkUrl;
        BankSyncBudgetIds = bankSyncBudgetIds;
        CustomCssPath = customCssPath;
        CustomJavascriptPath = customJavascriptPath;
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

        var bankSyncBudgetIds = env["BANK_SYNC_BUDGET_IDS"] as string;
        
        var actualLinkUrl = env["ACTUAL_LINK_URL"] as string;
        
        var customCssPath = env["CUSTOM_CSS_PATH"] as string;
        
        var customJavascriptPath = env["CUSTOM_JAVASCRIPT_PATH"] as string;

        return new EnvironmentVariables(
            apiUrl, apiKey,
            actualLinkUrl,
            ToGuidArray(bankSyncBudgetIds),
            customCssPath, customJavascriptPath);
    }

    private static Guid[] ToGuidArray(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return [];
        
        return value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(Guid.Parse)
            .ToArray();
    }
}