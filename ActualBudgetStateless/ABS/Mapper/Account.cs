using ABS.ActualWrapper.Data;
using ABS.Models;

namespace ABS.Mapper;

public static partial class Mapper
{
    public static AccountViewModel Map(Account input)
    {
        return new AccountViewModel
        {
            Id = input.Id,
            Name = input.Name,
            OffBudget = input.OffBudget,
            Closed = input.Closed,
            ClearedBalance = ToDollars(input.ClearedBalance),
            UnclearedBalance = ToDollars(input.UnclearedBalance),
            WorkingBalance = ToDollars(input.WorkingBalance)
        };
    }

    public static AccountViewModel[] Map(Account[] input)
    {
        return input.Select(Map).ToArray();
    }
}