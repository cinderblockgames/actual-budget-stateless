namespace ABS.ActualWrapper.Data;

public class MonthInfo
{
    public string Month { get; set; }
    public decimal IncomeAvailable { get; set; }
    public decimal LastMonthOverspent { get; set; }
    public decimal ForNextMonth { get; set; }
    public decimal TotalBudgeted { get; set; }
    public decimal ToBudget { get; set; }
    public decimal FromLastMonth { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal TotalBalance { get; set; }
    public IEnumerable<CategoryGroup> CategoryGroups { get; set; }
}