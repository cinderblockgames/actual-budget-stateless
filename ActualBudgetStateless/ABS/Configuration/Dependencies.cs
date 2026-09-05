using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ABS.Configuration;

public static class Dependencies
{
    public static void Load(IServiceCollection services)
    {
        var env = EnvironmentVariables.Build();
        services.AddSingleton(env);

        // Actual.
        services.AddSingleton(new ActualWrapper.ConnectionInfo
        {
            ApiUrl = env.ApiUrl,
            ApiKey = env.ApiKey,
            BudgetSyncId = Guid.Parse(env.BudgetSyncId)
        });
        services.AddSingleton<ActualWrapper.Actual>();
    }

    public static IEnumerable<Type> GetAllFilters()
    {
        return typeof(Dependencies).Assembly.GetTypes()
            .Where(type => type.IsAssignableTo(typeof(IFilterMetadata)))
            .Where(type => !type.IsAssignableTo(typeof(ControllerBase)));
    }
}