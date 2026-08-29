namespace ABS.Models;

public class TransactionViewModel
{
    public Guid Id { get; set; }
    public bool Parent { get; set; }
    public bool Child { get; set; }
    public Guid? ParentId { get; set; }
    public Guid AccountId { get; set; }
    public string Account { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Category { get; set; }
    public string? Amount { get; set; }
    public string? Payment { get; set; }
    public string? Deposit { get; set; }
    public Guid? PayeeId { get; set; }
    public string? Payee { get; set; }
    public string? PayeeTruncated { get; set; }
    public string? Notes { get; set; }
    public string? NotesTruncated { get; set; }
    public string Date { get; set; }
    public bool Cleared { get; set; }
    public bool Reconciled { get; set; }
    public IEnumerable<TransactionViewModel>? SubTransactions { get; set; }
}