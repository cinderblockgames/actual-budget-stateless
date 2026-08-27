namespace ABS.ActualWrapper.Query;

public class AccountFilter(Guid account)
{
    public Guid Account { get; set; } = account;
}