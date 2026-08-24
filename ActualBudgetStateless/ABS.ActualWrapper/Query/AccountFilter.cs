namespace ABS.ActualWrapper.Query;

public class AccountFilter(string account)
{
    public string Account { get; set; } = account;
}