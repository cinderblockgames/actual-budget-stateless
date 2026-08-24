using System.Text.Json.Serialization;

namespace ABS.ActualWrapper.Query;

public class Filter
{
    [JsonPropertyName("$or")]
    public IEnumerable<AccountFilter> Accounts { get; set; }

    public Filter(IEnumerable<string> accountIds)
    {
        Accounts = accountIds.Select(id => new AccountFilter(id));
    }
}