using ABS.ActualWrapper.Data;
using ABS.Models;

namespace ABS.Mapper;

public static partial class Mapper
{
    public static MonthInfoViewModel Map(MonthInfo input)
    {
        return new MonthInfoViewModel
        {
            Month = new MonthViewModel(input.Month),
            NextMonth = new MonthViewModel(input.Month, +1),
            PreviousMonth = new MonthViewModel(input.Month, -1),
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
}