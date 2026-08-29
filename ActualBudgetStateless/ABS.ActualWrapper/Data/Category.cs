namespace ABS.ActualWrapper.Data;

public class Category : CategoryStub
{
    public decimal Budgeted { get; set; }
    public decimal Spent { get; set; }
    public decimal Balance { get; set; }
    public decimal Received { get; set; }
    public bool Carryover { get; set; }
}