namespace ABS.Models;

public class CategoryViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool Income { get; set; }
    public bool Hidden { get; set; }
    public string GroupId { get; set; }
    public string Budgeted { get; set; }
    public string Spent { get; set; }
    public string Balance { get; set; }
    public string Received { get; set; }
    public bool Carryover { get; set; }
}