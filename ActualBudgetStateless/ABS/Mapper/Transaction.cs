using ABS.ActualWrapper.Data;
using ABS.Models;

namespace ABS.Mapper;

public static partial class Mapper
{
    public static TransactionViewModel Map(Transaction input)
    {
        return new TransactionViewModel
        {
            Id = input.Id,
            Parent = input.Is_Parent,
            Child = input.Is_Child,
            ParentId = input.ParentId,
            AccountId = input.Account,
            CategoryId = input.Category,
            Amount = ToDollars(input.Amount),
            PayeeId = input.Payee,
            Notes = input.Notes,
            Date = input.Date,
            Cleared = input.Cleared,
            Reconciled = input.Reconciled,
            SubTransactions = Map(input.SubTransactions)
        };
    }

    public static IEnumerable<TransactionViewModel>? Map(IEnumerable<Transaction>? input)
    {
        return input?.Select(Map);
    }
}