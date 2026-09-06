using ABS.ActualWrapper.Data;
using ABS.Models;

namespace ABS.Mapper;

public static partial class Mapper
{
    public static CategoryGroupViewModel Map(CategoryGroup input)
    {
        return new CategoryGroupViewModel
        {
            Id = input.Id,
            Name = input.Name,
            Income = input.Is_Income,
            Hidden = input.Hidden,
            Budgeted = ToDollars(input.Budgeted),
            Spent = ToDollars(input.Spent),
            Balance = ToDollars(input.Balance),
            Categories = Map(input.Categories)
        };
    }
    
    public static CategoryGroupViewModel[] Map(CategoryGroup[] input)
    {
        return input.Select(Map).ToArray();
    }
}