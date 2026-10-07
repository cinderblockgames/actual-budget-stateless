using ABS.ActualWrapper;

namespace ABS.Jobs;

internal class BankSyncJob(Actual actual, Guid[] budgetIds) : Job
{
    protected override async Task Process()
    {
        foreach (var budgetId in budgetIds)
        {
            Console.WriteLine($"Starting bank sync for {budgetId}.");
            await actual.BankSync(budgetId);
        }
    }
}