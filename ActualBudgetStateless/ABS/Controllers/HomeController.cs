using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ABS.Models;

namespace ABS.Controllers;

public class HomeController() : Controller
{
    public async Task<IActionResult> Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}