namespace ABS.ActualWrapper.Data;

public class CategoryGroup
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool Is_Income { get; set; }
    public bool Hidden { get; set; }
    public decimal Budgeted { get; set; }
    public decimal Spent { get; set; }
    public decimal Balance { get; set; }
    public IEnumerable<Category> Categories { get; set; }
}