namespace ABS.ActualWrapper.Data;

public class Transaction
{
    public Guid Id { get; set; }
    public bool Is_Parent { get; set; }
    public bool Is_Child { get; set; }
    public Guid? ParentId { get; set; }
    public Guid Account { get; set; }
    public Guid? Category { get; set; }
    public decimal? Amount { get; set; }
    public Guid? Payee { get; set; }
    public string Notes { get; set; }
    public string Date { get; set; }
    public string Imported_Id { get; set; }
    public bool Starting_Balance_Flag { get; set; }
    public string Transfer_Id { get; set; }
    public long Sort_Order { get; set; }
    public bool Cleared { get; set; }
    public bool Reconciled { get; set; }
    public bool Tombostone { get; set; }
    public string Schedule { get; set; }
    public IEnumerable<Transaction> SubTransactions { get; set; }
}