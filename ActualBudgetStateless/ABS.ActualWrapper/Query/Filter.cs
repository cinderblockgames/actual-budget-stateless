using System.Text.Json.Serialization;

namespace ABS.ActualWrapper.Query;

[JsonDerivedType(typeof(UncategorizedFilter))]
public class Filter
{
    [JsonPropertyName("$or")]
    public IEnumerable<AccountFilter> Accounts { get; set; }

    public Filter(IEnumerable<Guid> accountIds)
    {
        Accounts = accountIds.Select(id => new AccountFilter(id));
    }
}