namespace ABS.Models;

public class TransactionsViewModel
{
    public string Context { get; set; }
    public IEnumerable<TransactionViewModel> Transactions { get; set; }
}