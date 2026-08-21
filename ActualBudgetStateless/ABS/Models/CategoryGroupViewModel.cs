namespace ABS.Models;

public class CategoryGroupViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool Income { get; set; }
    public bool Hidden { get; set; }
    public string Budgeted { get; set; }
    public string Spent { get; set; }
    public string Balance { get; set; }
    public IEnumerable<CategoryViewModel> Categories { get; set; }
}