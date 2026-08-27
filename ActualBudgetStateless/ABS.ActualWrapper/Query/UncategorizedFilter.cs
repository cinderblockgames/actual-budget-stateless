using System.Text.Json.Serialization;

namespace ABS.ActualWrapper.Query;

public class UncategorizedFilter(IEnumerable<Guid> accountIds) : Filter(accountIds)
{
    [JsonPropertyName("category")]
    public Guid? CategoryId { get; set; } = null;

    [JsonPropertyName("transfer_id")]
    public Guid? TransferId { get; set; } = null;

    [JsonPropertyName("starting_balance_flag")]
    public bool StartingBalance { get; set; } = false;
}