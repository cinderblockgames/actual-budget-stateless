using Microsoft.AspNetCore.Mvc;
using ABS.ActualWrapper;
using ABS.ActualWrapper.Data;
using ABS.Models;
using static ABS.Configuration.Constants.Session;
using static ABS.Mapper.Mapper;

namespace ABS.Controllers;

public class AccountsController(Actual actual) : Controller
{
    private Guid BudgetId => new Guid(HttpContext.Session.Get(Keys.BudgetFile)!);
    
    public async Task<IActionResult> Index()
    {
        var accounts = await actual.GetAccounts(BudgetId);
        
        var openAccounts = accounts.Where(acc => !acc.Closed).ToArray();
        var onBudgetAccounts = openAccounts.Where(acc => !acc.OffBudget).ToArray();
        var offBudgetAccounts = openAccounts.Where(acc => acc.OffBudget).ToArray();
        var closedAccounts = accounts.Where(acc => acc.Closed).ToArray();

        var vm = new AccountsViewModel
        {
            OnBudgetAccounts = Map(onBudgetAccounts),
            OffBudgetAccounts = Map(offBudgetAccounts),
            ClosedAccounts = Map(closedAccounts),
            
            AllAccountsTotal = Sum(openAccounts),
            OnBudgetAccountsTotal = Sum(onBudgetAccounts),
            OffBudgetAccountsTotal = Sum(offBudgetAccounts)
        };
        return View(vm);
    }

    private string Sum(Account[] accounts)
    {
        return ToDollars(accounts.Sum(acc => acc.WorkingBalance)) ?? "0";
    }
}