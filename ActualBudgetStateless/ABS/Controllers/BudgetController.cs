using System.Text;
using Microsoft.AspNetCore.Mvc;
using ABS.ActualWrapper;
using ABS.ActualWrapper.Data;
using static ABS.Mapper.Mapper;
using static ABS.Configuration.Constants.Session;

namespace ABS.Controllers;

public class BudgetController(Actual actual) : Controller
{

    private Guid BudgetId => new Guid(HttpContext.Session.Get(Keys.BudgetFile)!);
    
    [Route("budget/{year}/{month}")]
    public async Task<IActionResult> Index(int year, int month)
    {
        var summaryTask = actual.GetMonthInfo(BudgetId, year, month);
        var notesTask = GetNotesForCategories();
        var automationsTask = actual.GetAutomationsForCategories(BudgetId);
        var uncategorizedTask = Uncategorized();
        
        var summary = await summaryTask;
        if (summary == null)
        {
            return RedirectToAction(nameof(Index), new { year = DateTime.Now.Year, month = DateTime.Now.Month });
        }

        var mapped = Map(summary);
        var available = await actual.GetMonths(BudgetId);

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
        mapped.Overspent = Overspent(summary.CategoryGroups?.SelectMany(cg => cg.Categories));

        return View(mapped);
    }

    private async Task<Dictionary<Guid, string>> GetNotesForCategories()
    {
        var categories = await actual.GetCategories(BudgetId);
        var tasks = categories.ToDictionary(
            c => c.Id,
            c => actual.GetNotesForCategory(BudgetId, c.Id));
        return tasks.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.GetAwaiter().GetResult());
    }

    private async Task<string?> Uncategorized()
    {
        var accounts = await actual.GetAccounts(BudgetId);
        var transactions = await actual.GetUncategorizedTransactions(
            BudgetId,
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

    private string? Overspent(IEnumerable<Category>? categories)
    {
        var overspent = categories
            ?.Where(c => c is { Hidden: false, Is_Income: false, Carryover: false })
            .Where(c => c.Balance < 0)
            .ToArray();
        if (overspent?.Any() == true)
        {
            var sb = new StringBuilder("You have ");
            sb.Append(overspent.Length);
            sb.Append(" overspent ");
            sb.Append(overspent.Length > 1 ? "categories" : "category");
            sb.Append(" (");
            sb.Append(ToDollars(overspent.Sum(c => c.Balance)));
            sb.Append(").");
            return sb.ToString();
        }
        
        return null;
    }
    
}