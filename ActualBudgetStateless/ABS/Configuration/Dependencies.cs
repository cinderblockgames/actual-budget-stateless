namespace ABS.Configuration;

public static class Dependencies
{
    public static void Load(IServiceCollection services)
    {
        var env = EnvironmentVariables.Build();

        // Actual.
        services.AddSingleton(new ActualWrapper.ConnectionInfo
        {
            ApiUrl = env.ApiUrl,
            ApiKey = env.ApiKey,
            BudgetSyncId = Guid.Parse(env.BudgetSyncId)
        });
        services.AddSingleton<ActualWrapper.Actual>();
    }
}