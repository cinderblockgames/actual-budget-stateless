using System.Text.Json.Serialization;

namespace ABS.ActualWrapper.Query;

public class UncategorizedFilter(IEnumerable<string> accountIds) : Filter(accountIds)
{
    [JsonPropertyName("category")]
    public string? CategoryId { get; set; } = null;

    [JsonPropertyName("transfer_id")]
    public string? TransferId { get; set; } = null;

    [JsonPropertyName("starting_balance_flag")]
    public bool StartingBalance { get; set; } = false;
}