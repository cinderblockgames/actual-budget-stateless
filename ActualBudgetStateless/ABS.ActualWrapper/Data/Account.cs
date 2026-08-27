namespace ABS.ActualWrapper.Data;

public class Account
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool OffBudget { get; set; }
    public bool Closed { get; set; }
    public decimal ClearedBalance { get; set; }
    public decimal UnclearedBalance { get; set; }
    public decimal WorkingBalance { get; set; }
}