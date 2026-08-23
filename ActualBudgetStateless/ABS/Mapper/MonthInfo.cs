using ABS.ActualWrapper.Data;
using ABS.Models;

namespace ABS.Mapper;

public static partial class Mapper
{
    public static MonthInfoViewModel Map(MonthInfo input)
    {
        var split = SplitMonth(input.Month);
        return new MonthInfoViewModel
        {
            Month = split.Month,
            Year = split.Year,
            IncomeAvailable = ToDollars(input.IncomeAvailable),
            LastMonthOverspent = ToDollars(input.LastMonthOverspent),
            ForNextMonth = ToDollars(input.ForNextMonth),
            // Budgeted comes in negative to show it's spending money
            TotalBudgeted = ToDollars(-1*input.TotalBudgeted),
            ToBudget = ToDollars(input.ToBudget),
            FromLastMonth = ToDollars(input.FromLastMonth),
            TotalIncome = ToDollars(input.TotalIncome),
            TotalSpent = ToDollars(input.TotalSpent),
            TotalBalance = ToDollars(input.TotalBalance),
            CategoryGroups = Map(input.CategoryGroups)
        };
    }

    private static (string Month, int Year) SplitMonth(string input)
    {
        var dt = DateTime.Parse(input);
        return (dt.ToString("MMMM"), dt.Year);
    }
}