using System.Net.Http.Json;
using ABS.ActualWrapper.Data;

namespace ABS.ActualWrapper;

public class Actual
{
    
    #region " Constructor, Private Properties "
    
    private HttpClient Api { get; }

    public Actual(ConnectionInfo connectionInfo)
    {
        var handler = new HttpClientHandler();
        handler.ClientCertificateOptions = ClientCertificateOption.Manual;
        handler.ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) => true;
        
        #if DEBUG
        Api = new HttpClient(handler)
        #else
        Api = new HttpClient()
        #endif
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
        return await Process<MonthInfo>(() => Api.GetAsync($"months/{year}-{month:00}"));
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