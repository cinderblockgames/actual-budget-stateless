using System.Diagnostics;
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
        return View(Map(summary));
    }
}