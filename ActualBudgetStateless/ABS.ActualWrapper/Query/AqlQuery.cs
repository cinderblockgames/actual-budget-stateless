namespace ABS.ActualWrapper.Query;

public class AqlQuery
{
    public string Table { get; set; }
    public Filter Filter { get; set; }
    public IEnumerable<string> Select { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
}