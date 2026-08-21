using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ABS.ActualWrapper;
using ABS.Models;
using static ABS.Mapper.Mapper;

namespace ABS.Controllers;

public class HomeController(Actual actual) : Controller
{
    public async Task<IActionResult> Index()
    {
        var summary = await actual.GetMonthInfo(2026, 8);
        return View(Map(summary));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}