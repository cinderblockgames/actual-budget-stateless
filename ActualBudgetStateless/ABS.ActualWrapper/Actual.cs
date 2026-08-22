using System.Net.Http.Json;
using ABS.ActualWrapper.Data;

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

    public async Task<MonthInfo> GetMonthInfo(int year, int month)
    {
        return await Process<MonthInfo>(() =>
            Api.GetAsync($"months/{year}-{month:00}"
        ));
    }

    public async Task<IEnumerable<Account>> GetAccounts()
    {
        return await Process<IEnumerable<Account>>(() =>
            Api.GetAsync("accounts?include_balances=true&exclude_offbudget=false&exclude_closed=false"
        ));
    }
    
    #region " Process "

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