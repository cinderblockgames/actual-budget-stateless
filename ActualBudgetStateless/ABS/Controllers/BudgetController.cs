using Microsoft.AspNetCore.Mvc;
using ABS.ActualWrapper;
using ABS.Configuration;
using static ABS.Mapper.Mapper;

namespace ABS.Controllers;

public class BudgetController : Controller
{
    
    #region " Constructor, Private Properties "
    
    private readonly Actual _actual;
    private readonly Cache<IEnumerable<string>> _months;
    
    public BudgetController(Actual actual)
    {
        _actual = actual;
        _months = new(_actual.GetMonths, TimeSpan.FromHours(1));
    }
    
    #endregion
    
    [Route("budget/{year}/{month}")]
    public async Task<IActionResult> Index(int year, int month)
    {
        var summary = await _actual.GetMonthInfo(year, month);
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

        return View(mapped);
    }
}