using Microsoft.AspNetCore.Mvc;
using ABS.ActualWrapper;
using ABS.ActualWrapper.Data;
using ABS.Configuration;
using ABS.Models;
using static ABS.Mapper.Mapper;

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
    public async Task<IActionResult> Account(string accountId, int page = 1)
    {
        return await Process(
            pg => _actual.GetTransactions(accountId, pg),
            page);
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
            page);
    }

    [Route("transactions")]
    [Route("transactions/{page}")]
    public async Task<IActionResult> All(int page = 1)
    {
        return await ProcessAccountsSubset(
            acct => !acct.Closed,
            page);
    }
    
    [Route("transactions/onbudget")]
    [Route("transactions/onbudget/{page}")]
    public async Task<IActionResult> OnBudget(int page = 1)
    {
        return await ProcessAccountsSubset(
            acct => !acct.Closed && !acct.OffBudget,
            page);
    }
    
    [Route("transactions/offbudget")]
    [Route("transactions/offbudget/{page}")]
    public async Task<IActionResult> OffBudget(int page = 1)
    {
        return await ProcessAccountsSubset(
            acct => !acct.Closed && acct.OffBudget,
            page);
    }
    
    #region " Process "

    private async Task<IActionResult> ProcessAccountsSubset(Func<Account, bool> selector, int page)
    {
        var accountIds = (await _accounts.GetValue())
            .Where(selector)
            .Select(acct => acct.Id);
        return await Process(
            pg => _actual.GetTransactions(accountIds, pg),
            page);
    }

    private static readonly IEnumerable<TransactionViewModel> EMPTY =
        Enumerable.Empty<TransactionViewModel>();
    private async Task<IActionResult> Process(Func<int, Task<IEnumerable<Transaction>>> retriever, int page)
    {
        if (page < 1)
        {
            return Json(EMPTY);
        }

        var transactions = await GetTransactions(retriever(page));

        if (transactions?.Any() != true)
        {
            return page > 1 ? Json(EMPTY) : View(EMPTY);
        }

        var mapped = await MapTransactions(transactions);
        return page > 1 ? Json(mapped) : View(mapped);
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
            if (transaction.AccountId != null)
                transaction.Account = accountMap[transaction.AccountId];
            if (transaction.CategoryId != null)
                transaction.Category = categoryMap[transaction.CategoryId];
            if (transaction.PayeeId != null)
                transaction.Payee = payeeMap[transaction.PayeeId];
            transaction.PayeeTruncated = Truncate(transaction.Payee);
            transaction.NotesTruncated = Truncate(transaction.Notes);
        }

        return mapped;
    }
    
    #endregion
    
}