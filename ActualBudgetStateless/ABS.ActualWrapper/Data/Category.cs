namespace ABS.ActualWrapper.Data;

public class Category
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool Is_Income { get; set; }
    public bool Hidden { get; set; }
    public string Group_Id { get; set; }
    public decimal Budgeted { get; set; }
    public decimal Spent { get; set; }
    public decimal Balance { get; set; }
    public decimal Received { get; set; }
    public bool Carryover { get; set; }
}