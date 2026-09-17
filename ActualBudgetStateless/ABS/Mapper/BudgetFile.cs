using ABS.ActualWrapper.Data;
using ABS.Models;

namespace ABS.Mapper;

public static partial class Mapper
{
    public static BudgetFileViewModel Map(BudgetFileStub input)
    {
        return new BudgetFileViewModel
        {
            Id = input.GroupId,
            Name = input.Name
        };
    }

    public static BudgetFileViewModel[] Map(BudgetFileStub[] input)
    {
        return input.Select(Map).ToArray();
    }
}