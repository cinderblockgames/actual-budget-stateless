namespace ABS.Models;

public class TransactionViewModel
{
    public string Id { get; set; }
    public bool Parent { get; set; }
    public bool Child { get; set; }
    public string? ParentId { get; set; }
    public string? AccountId { get; set; }
    public string? Account { get; set; }
    public string? CategoryId { get; set; }
    public string? Category { get; set; }
    public string? Payment { get; set; }
    public string? Deposit { get; set; }
    public string? PayeeId { get; set; }
    public string? Payee { get; set; }
    public string? Notes { get; set; }
    public string Date { get; set; }
    public bool Cleared { get; set; }
    public bool Reconciled { get; set; }
    public IEnumerable<TransactionViewModel>? SubTransactions { get; set; }
}