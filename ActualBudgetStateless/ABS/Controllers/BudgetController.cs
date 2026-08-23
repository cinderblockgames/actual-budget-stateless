using Microsoft.AspNetCore.Mvc;
using ABS.ActualWrapper;
using static ABS.Mapper.Mapper;

namespace ABS.Controllers;

public class BudgetController(Actual actual) : Controller
{
    [Route("Budget/{year}/{month}")]
    public async Task<IActionResult> Index(int year, int month)
    {
        var summary = await actual.GetMonthInfo(year, month);
        if (summary == null)
        {
            return RedirectToAction("Index", new { year = DateTime.Now.Year, month = DateTime.Now.Month });
        }

        var mapped = Map(summary);
        var available = await GetMonths();

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

    #region " GetMonths "

    private static IEnumerable<string> _months;
    private static DateTime _expires;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    private async Task<IEnumerable<string>> GetMonths()
    {
        if (_months == null || DateTime.Now > _expires)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_months == null || DateTime.Now > _expires)
                {
                    _months = await actual.GetMonths();
                    _expires = DateTime.Now.AddHours(1); // Cache for one hour.
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        return _months;
    }

    #endregion
}