using System.Net.Http.Json;
using ABS.ActualWrapper.Data;
using ABS.ActualWrapper.Query;

namespace ABS.ActualWrapper;

public class Actual
{
    
    #region " Constructor, Private Properties "
    
    private HttpClient Api { get; }

    public Actual(ConnectionInfo connectionInfo)
    {
        Api = new HttpClient()
        {
            BaseAddress = new Uri(
                new Uri(connectionInfo.ApiUrl!),
                $"v1/budgets/{connectionInfo.BudgetSyncId!}/"
            )
        };
        Api.DefaultRequestHeaders.Add("X-API-KEY", connectionInfo.ApiKey!);
    }
    
    #endregion

    public async Task<CategoryStub[]> GetCategories()
    {
        return await Process<CategoryStub[]>(() =>
            Api.GetAsync("categories")
        );
    }
    
    public async Task<Payee[]> GetPayees()
    {
        return await Process<Payee[]>(() =>
            Api.GetAsync("payees")
        );
    }
    
    public async Task<string[]> GetMonths()
    {
        return await Process<string[]>(() =>
            Api.GetAsync("months")
        );
    }
    
    public async Task<MonthInfo> GetMonthInfo(int year, int month)
    {
        return await Process<MonthInfo>(() =>
            Api.GetAsync($"months/{year}-{month:00}")
        );
    }

    public async Task<Account[]> GetAccounts()
    {
        return await Process<Account[]>(() =>
            Api.GetAsync("accounts?include_balances=true&exclude_offbudget=false&exclude_closed=false")
        );
    }

    public async Task<Transaction[]> GetTransactions(Guid accountId, int page)
    {
        return await Process<Transaction[]>(() =>
            Api.GetAsync($"accounts/{accountId}/transactions?since_date=1970-01-01&limit=50&page={page}")
        );
    }

    public async Task<Transaction[]> GetTransactions(IEnumerable<Guid> accountIds, int page)
    {
        return await ProcessTransactionsRequest(new Filter(accountIds), page);
    }

    public async Task<Transaction[]> GetUncategorizedTransactions(IEnumerable<Guid> accountIds, int page)
    {
        return await ProcessTransactionsRequest(new UncategorizedFilter(accountIds), page);
    }

    public async Task<string> GetNotesForCategory(Guid categoryId)
    {
        return await Process<string>(() =>
            Api.GetAsync($"notes/category/{categoryId}")
        );
    }
    
    #region " Process "

    private async Task<Transaction[]> ProcessTransactionsRequest(Filter filter, int page)
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
            Api.PostAsJsonAsync("run-query", request)
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

            Console.WriteLine($"    [{(int)response.StatusCode} {response.StatusCode}] {await response.Content.ReadAsStringAsync()}");
            return default;
        }

        var wrapper = await response.Content.ReadFromJsonAsync<DataWrapper<T>>();
        return wrapper.Data;
    }
    
    #endregion

}