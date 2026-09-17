using System.Net.Http.Json;
using System.Text.Json;
using ABS.ActualWrapper.Data;
using ABS.ActualWrapper.Query;

namespace ABS.ActualWrapper;

// Singleton
public class Actual
{
    
    public BudgetFileStub[] BudgetFiles { get; }

    #region " Constructor, Private Properties "

    private Uri ApiUrl { get; }
    private HttpClient Api { get; }
    
    public Actual(ConnectionInfo connectionInfo)
    {
        ApiUrl = new Uri(connectionInfo.ApiUrl!);
        Api = new HttpClient { BaseAddress = new Uri(ApiUrl, "v1/budgets/") };
        Api.DefaultRequestHeaders.Add("X-API-KEY", connectionInfo.ApiKey!);
            
        // Pre-fetch budget file stubs.
        BudgetFiles = FillBudgetFiles().GetAwaiter().GetResult();
    }
    
    private async Task<BudgetFileStub[]> FillBudgetFiles()
    {
        var files = await Process<BudgetFileStub[]>(() =>
            Api.GetAsync("")
        );
        return files.Where(file => "remote".Equals(file.State, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
    
    #endregion
    
    public async Task<CategoryStub[]> GetCategories(Guid budgetId)
    {
        return await Process<CategoryStub[]>(() =>
            Api.GetAsync($"{budgetId}/categories")
        );
    }
    
    public async Task<Payee[]> GetPayees(Guid budgetId)
    {
        return await Process<Payee[]>(() =>
            Api.GetAsync($"{budgetId}/payees")
        );
    }
    
    public async Task<string[]> GetMonths(Guid budgetId)
    {
        return await Process<string[]>(() =>
            Api.GetAsync($"{budgetId}/months")
        );
    }
    
    public async Task<MonthInfo> GetMonthInfo(Guid budgetId, int year, int month)
    {
        return await Process<MonthInfo>(() =>
            Api.GetAsync($"{budgetId}/months/{year}-{month:00}")
        );
    }

    public async Task<Account[]> GetAccounts(Guid budgetId)
    {
        return await Process<Account[]>(() =>
            Api.GetAsync($"{budgetId}/accounts?include_balances=true&exclude_offbudget=false&exclude_closed=false")
        );
    }

    public async Task<Transaction[]> GetTransactions(Guid budgetId, Guid accountId, int page)
    {
        return await Process<Transaction[]>(() =>
            Api.GetAsync($"{budgetId}/accounts/{accountId}/transactions?since_date=1970-01-01&limit=50&page={page}")
        );
    }

    public async Task<Transaction[]> GetTransactions(Guid budgetId, IEnumerable<Guid> accountIds, int page)
    {
        return await ProcessTransactionsRequest(budgetId, new Filter(accountIds), page);
    }

    public async Task<Transaction[]> GetUncategorizedTransactions(Guid budgetId, IEnumerable<Guid> accountIds, int page)
    {
        return await ProcessTransactionsRequest(budgetId, new UncategorizedFilter(accountIds), page);
    }

    public async Task<string> GetNotesForCategory(Guid budgetId, Guid categoryId)
    {
        return await Process<string>(() =>
            Api.GetAsync($"{budgetId}/notes/category/{categoryId}")
        );
    }

    public async Task<Dictionary<Guid, GoalDefinition[]>> GetAutomationsForCategories(Guid budgetId)
    {
        var request = new Wrapper
        {
            AqlQuery = new AqlQuery
            {
                Table = "categories",
                Select = ["id", "goal_def"]
            }
        };

        var response = await Process<CategoryGoalDefinition[]>(() =>
            Api.PostAsJsonAsync($"{budgetId}/run-query", request)
        );
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var dict = new Dictionary<Guid, GoalDefinition[]>();
        foreach (var value in response)
        {
            if (value.Goal_Def != null)
            {
                var def = JsonSerializer.Deserialize<GoalDefinition[]>(value.Goal_Def, options);
                dict[value.Id] = def!;
            }
        }

        return dict;
    }
    
    #region " Process "

    private async Task<Transaction[]> ProcessTransactionsRequest(Guid budgetId, Filter filter, int page)
    {
        var request = new Wrapper
        {
            AqlQuery = new AqlQuery
            {
                Table = "transactions",
                Filter = filter,
                Select = ["*"],
                Limit = 50,
                Offset = 50*(page-1)
            }
        };

        return await Process<Transaction[]>(() =>
            Api.PostAsJsonAsync($"{budgetId}/run-query", request)
        );
    }

    private async Task<T> Process<T>(Func<Task<HttpResponseMessage>> call)
    {
        var response = await call();
        if (!response.IsSuccessStatusCode)
        {
            var request = response.RequestMessage?.Content;
            if (request != null)
            {
                Console.WriteLine(await request.ReadAsStringAsync());
            }

            Console.WriteLine(
                $"    [{(int)response.StatusCode} {response.StatusCode}] {await response.Content.ReadAsStringAsync()}");
            return default;
        }

        var wrapper = await response.Content.ReadFromJsonAsync<DataWrapper<T>>();
        return wrapper.Data;
    }
    
    #endregion

    private class CategoryGoalDefinition
    {
        public Guid Id { get; set; }
        public string? Goal_Def { get; set; }
    }

}