using Microsoft.AspNetCore.Mvc;
using ABS.ActualWrapper;
using ABS.ActualWrapper.Data;
using ABS.Configuration;
using ABS.Models;
using static ABS.Mapper.Mapper;

namespace ABS.Controllers;

public class TransactionsController : Controller
{
    
    #region " Constructor, Private Properties "
    
    private readonly Actual _actual;
    private readonly Cache<IEnumerable<Account>> _accounts;
    private readonly Cache<IEnumerable<CategoryStub>> _categories;
    private readonly Cache<IEnumerable<Payee>> _payees;
    
    public TransactionsController(Actual actual)
    {
        _actual = actual;

        var validity = TimeSpan.FromMinutes(2); // Just doing a quick cache for processing.
        _accounts = new(_actual.GetAccounts, validity);
        _categories = new(_actual.GetCategories, validity);
        _payees = new(_actual.GetPayees, validity);
    }
    
    #endregion
    
    [Route("transactions/account/{accountId}")]
    [Route("transactions/account/{accountId}/{page}")]
    public async Task<IActionResult> Account(Guid accountId, int page = 1)
    {
        return await Process(
            pg => _actual.GetTransactions(accountId, pg),
            page,
            () =>
            {
                var accounts = _accounts.GetValue().GetAwaiter().GetResult(); // Already cached.
                var account = accounts.SingleOrDefault(acct => acct.Id == accountId);
                return account?.Name;
            });
    }
    
    [Route("transactions/uncategorized")]
    [Route("transactions/uncategorized/{page}")]
    public async Task<IActionResult> Uncategorized(int page = 1)
    {
        var accountIds = (await _accounts.GetValue())
            .Where(acct => !acct.Closed)
            .Select(acct => acct.Id);
        return await Process(
            pg => _actual.GetUncategorizedTransactions(accountIds, pg),
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
        var accountIds = (await _accounts.GetValue())
            .Where(selector)
            .Select(acct => acct.Id);
        return await Process(
            pg => _actual.GetTransactions(accountIds, pg),
            page,
            contextRetriever);
    }

    private static readonly IEnumerable<TransactionViewModel> EMPTY =
        Enumerable.Empty<TransactionViewModel>();
    private async Task<IActionResult> Process(
        Func<int, Task<IEnumerable<Transaction>>> retriever,
        int page,
        Func<string?> contextRetriever)
    {
        if (page < 1)
        {
            return Json(EMPTY);
        }

        var transactions = await GetTransactions(retriever(page));
        var context = contextRetriever();
        
        if (transactions?.Any() != true)
        {
            return ViewOrJson(EMPTY, page, contextRetriever);
        }

        var mapped = await MapTransactions(transactions);
        return ViewOrJson(mapped, page, contextRetriever);
    }

    private IActionResult ViewOrJson(
        IEnumerable<TransactionViewModel> transactions,
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

    private async Task<IEnumerable<Transaction>> GetTransactions(Task<IEnumerable<Transaction>> retriever)
    {
        // Pre-fill caches.
        var accountsTask = _accounts.GetValue();
        var categoriesTask = _categories.GetValue();
        var payeesTask = _payees.GetValue();
        await Task.WhenAll(retriever, accountsTask, categoriesTask, payeesTask);
        return await retriever;
    }

    private async Task<IEnumerable<TransactionViewModel>> MapTransactions(IEnumerable<Transaction> transactions)
    {
        var accountMap = (await _accounts.GetValue()).ToDictionary(
            a => a.Id,
            a => a.Name);
        var categoryMap = (await _categories.GetValue()).ToDictionary(
            c => c.Id,
            c => c.Name);
        var payeeMap = (await _payees.GetValue()).ToDictionary(
            p => p.Id,
            p => p.Name);

        var mapped = Map(transactions).ToArray(); // Needed so the below sticks.
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