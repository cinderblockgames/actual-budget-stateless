using ABS.ActualWrapper;
using ABS.ActualWrapper.Data;
using ABS.Models;
using Microsoft.AspNetCore.Mvc;
using static ABS.Configuration.Constants.Session;
using static ABS.Mapper.Mapper;

namespace ABS.Controllers;

public class TransactionsController(Actual actual) : Controller
{

    private Guid BudgetId => new Guid(HttpContext.Session.Get(Keys.BudgetFile)!);
    
    [Route("transactions/account/{accountId}")]
    [Route("transactions/account/{accountId}/{page}")]
    public async Task<IActionResult> Account(Guid accountId, int page = 1)
    {
        return await Process(
            pg => actual.GetTransactions(BudgetId, accountId, pg),
            page,
            () =>
            {
                var accounts = actual.GetAccounts(BudgetId).GetAwaiter().GetResult();
                var account = accounts.SingleOrDefault(acct => acct.Id == accountId);
                return account?.Name;
            });
    }
    
    [Route("transactions/uncategorized")]
    [Route("transactions/uncategorized/{page}")]
    public async Task<IActionResult> Uncategorized(int page = 1)
    {
        var accountIds = (await actual.GetAccounts(BudgetId))
            .Where(acct => acct is { Closed: false, OffBudget: false })
            .Select(acct => acct.Id);
        return await Process(
            pg => actual.GetUncategorizedTransactions(BudgetId, accountIds, pg),
            page,
            () => "Uncategorized Transactions");
    }

    [Route("transactions")]
    [Route("transactions/{page}")]
    public async Task<IActionResult> All(int page = 1)
    {
        return await ProcessAccountsSubset(
            acct => !acct.Closed,
            page,
            () => "All Accounts");
    }
    
    [Route("transactions/onbudget")]
    [Route("transactions/onbudget/{page}")]
    public async Task<IActionResult> OnBudget(int page = 1)
    {
        return await ProcessAccountsSubset(
            acct => !acct.Closed && !acct.OffBudget,
            page,
            () => "On Budget");
    }
    
    [Route("transactions/offbudget")]
    [Route("transactions/offbudget/{page}")]
    public async Task<IActionResult> OffBudget(int page = 1)
    {
        return await ProcessAccountsSubset(
            acct => !acct.Closed && acct.OffBudget,
            page,
            () => "Off Budget");
    }
    
    #region " Process "

    private async Task<IActionResult> ProcessAccountsSubset(
        Func<Account, bool> selector,
        int page,
        Func<string?> contextRetriever)
    {
        var accountIds = (await actual.GetAccounts(BudgetId))
            .Where(selector)
            .Select(acct => acct.Id);
        return await Process(
            pg => actual.GetTransactions(BudgetId, accountIds, pg),
            page,
            contextRetriever);
    }

    private static readonly TransactionViewModel[] EMPTY = [];
    private async Task<IActionResult> Process(
        Func<int, Task<Transaction[]>> retriever,
        int page,
        Func<string?> contextRetriever)
    {
        if (page < 1)
        {
            return Json(EMPTY);
        }

        var transactions = await retriever(page);
        
        if (transactions?.Any() != true)
        {
            return ViewOrJson(EMPTY, page, contextRetriever);
        }

        var mapped = await MapTransactions(transactions);
        return ViewOrJson(mapped, page, contextRetriever);
    }

    private IActionResult ViewOrJson(
        TransactionViewModel[] transactions,
        int page,
        Func<string> contextRetriever)
    {
        if (page > 1)
        {
            return Json(transactions);
        }

        var vm = new TransactionsViewModel
        {
            Transactions = transactions,
            Context = contextRetriever()
        };
        return View(vm);
    }

    private async Task<TransactionViewModel[]> MapTransactions(Transaction[] transactions)
    {
        var accountMap = (await actual.GetAccounts(BudgetId)).ToDictionary(
            a => a.Id,
            a => a.Name);
        var categoryMap = (await actual.GetCategories(BudgetId)).ToDictionary(
            c => c.Id,
            c => c.Name);
        var payeeMap = (await actual.GetPayees(BudgetId)).ToDictionary(
            p => p.Id,
            p => p.Name);

        var mapped = Map(transactions) ?? [];
        foreach (var transaction in mapped)
        {
            transaction.Account = accountMap[transaction.AccountId];
            if (transaction.CategoryId.HasValue)
                transaction.Category = categoryMap[transaction.CategoryId.Value];
            if (transaction.PayeeId.HasValue)
                transaction.Payee = payeeMap[transaction.PayeeId.Value];
            transaction.PayeeTruncated = Truncate(transaction.Payee);
            transaction.NotesTruncated = Truncate(transaction.Notes);
        }

        return mapped;
    }
    
    #endregion
    
}