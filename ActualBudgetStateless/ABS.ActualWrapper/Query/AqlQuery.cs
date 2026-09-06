namespace ABS.ActualWrapper.Query;

public class AqlQuery
{
    public required string Table { get; set; }
    public Filter? Filter { get; set; }
    public string[]? Select { get; set; } // {"amount":{"$sum":"$amount"}}]}
    public int? Limit { get; set; }
    public int? Offset { get; set; }
}