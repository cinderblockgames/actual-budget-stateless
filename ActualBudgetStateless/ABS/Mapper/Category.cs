using ABS.ActualWrapper.Data;
using ABS.Models;

namespace ABS.Mapper;

public static partial class Mapper
{
    public static CategoryViewModel Map(Category input)
    {
        return new CategoryViewModel
        {
            Id = input.Id,
            Name = input.Name,
            Income = input.Is_Income,
            Hidden = input.Hidden,
            GroupId = input.Group_Id,
            Budgeted = ToDollars(input.Budgeted),
            Spent = ToDollars(input.Spent),
            Balance = ToDollars(input.Balance),
            Received = ToDollars(input.Received),
            Carryover = input.Carryover
        };
    }

    public static CategoryViewModel[] Map(Category[] input)
    {
        return input.Select(Map).ToArray();
    }
}