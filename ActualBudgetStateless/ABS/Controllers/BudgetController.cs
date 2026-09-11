using System.Text;
using Microsoft.AspNetCore.Mvc;
using ABS.ActualWrapper;
using ABS.Configuration;
using static ABS.Mapper.Mapper;

namespace ABS.Controllers;

public class BudgetController : Controller
{
    
    #region " Constructor, Private Properties "
    
    private readonly Actual _actual;
    private readonly Cache<string[]> _months;
    
    public BudgetController(Actual actual)
    {
        _actual = actual;
        _months = new(_actual.GetMonths, TimeSpan.FromHours(1));
    }
    
    #endregion
    
    [Route("budget/{year}/{month}")]
    public async Task<IActionResult> Index(int year, int month)
    {
        var summaryTask = _actual.GetMonthInfo(year, month);
        var notesTask = GetNotesForCategories();
        var automationsTask = _actual.GetAutomationsForCategories();
        var uncategorizedTask = Uncategorized();
        
        var summary = await summaryTask;
        if (summary == null)
        {
            return RedirectToAction(nameof(Index), new { year = DateTime.Now.Year, month = DateTime.Now.Month });
        }

        var mapped = Map(summary);
        var available = await _months.GetValue();

        if (!available.Contains(mapped.PreviousMonth.Joined))
        {
            mapped.PreviousMonth = null;
        }

        if (!available.Contains(mapped.NextMonth.Joined))
        {
            mapped.NextMonth = null;
        }

        var notes = await notesTask;
        var automations = await automationsTask;
        foreach (var group in mapped.CategoryGroups)
        {
            foreach (var category in group.Categories)
            {
                if (notes.TryGetValue(category.Id, out var note))
                {
                    category.Notes = note;
                }

                if (automations.TryGetValue(category.Id, out var automation))
                {
                    category.Automation = Map(automation);
                }
            }
        }

        mapped.Uncategorized = await uncategorizedTask;

        return View(mapped);
    }

    private async Task<Dictionary<Guid, string>> GetNotesForCategories()
    {
        var categories = await _actual.GetCategories();
        var tasks = categories.ToDictionary(
            c => c.Id,
            c => _actual.GetNotesForCategory(c.Id));
        return tasks.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.GetAwaiter().GetResult());
    }

    private async Task<string?> Uncategorized()
    {
        var accounts = await _actual.GetAccounts();
        var transactions = await _actual.GetUncategorizedTransactions(
            accounts
                .Where(acct => !acct.Closed && !acct.OffBudget)
                .Select(acct => acct.Id),
            1);
        if (transactions?.Any() == true)
        {
            if (transactions.Length == 50)
            {
                return "You have 50+ uncategorized transactions.";
            }

            var sb = new StringBuilder("You have ");
            sb.Append(transactions.Length);
            sb.Append(" uncategorized transaction");
            if (transactions.Length > 1) sb.Append('s');
            sb.Append(" (");
            sb.Append(ToDollars(transactions.Sum(trx => trx.Amount ?? 0)));
            sb.Append(").");
            return sb.ToString();
        }

        return null;
    }
    
}