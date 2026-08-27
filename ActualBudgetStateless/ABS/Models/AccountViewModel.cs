namespace ABS.Models;

public class AccountViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool OffBudget { get; set; }
    public bool Closed { get; set; }
    public string ClearedBalance { get; set; }
    public string UnclearedBalance { get; set; }
    public string WorkingBalance { get; set; }
}