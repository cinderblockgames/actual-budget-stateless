namespace ABS.Models;

public class MonthInfoViewModel
{
    public MonthViewModel Month { get; set; }
    public MonthViewModel NextMonth { get; set; }
    public MonthViewModel PreviousMonth { get; set; }
    public bool IsCurrentMonth { get; set; }
    public string IncomeAvailable { get; set; }
    public string LastMonthOverspent { get; set; }
    public string ForNextMonth { get; set; }
    public string TotalBudgeted { get; set; }
    public string ToBudget { get; set; }
    public string FromLastMonth { get; set; }
    public string TotalIncome { get; set; }
    public string TotalSpent { get; set; }
    public string TotalBalance { get; set; }
    public IEnumerable<CategoryGroupViewModel> CategoryGroups { get; set; }
}