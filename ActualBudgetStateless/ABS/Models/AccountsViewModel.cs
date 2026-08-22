namespace ABS.Models;

public class AccountsViewModel
{
    public IEnumerable<AccountViewModel> OnBudgetAccounts { get; set; }
    public IEnumerable<AccountViewModel> OffBudgetAccounts { get; set; }
    public IEnumerable<AccountViewModel> ClosedAccounts { get; set; }
    public string AllAccountsTotal { get; set; }
    public string OnBudgetAccountsTotal { get; set; }
    public string OffBudgetAccountsTotal { get; set; }
}