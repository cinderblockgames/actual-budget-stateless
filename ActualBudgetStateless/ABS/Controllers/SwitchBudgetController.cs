using ABS.ActualWrapper;
using Microsoft.AspNetCore.Mvc;
using static ABS.Configuration.Constants.Session;
using static ABS.Mapper.Mapper;

namespace ABS.Controllers;

public class SwitchBudgetController(Actual actual) : Controller
{
    public IActionResult Index()
    {
        var vm = Map(actual.BudgetFiles);
        return View(vm);
    }

    public IActionResult SetBudgetFile(Guid id)
    {
        if (actual.BudgetFiles.Any(bf => bf.GroupId == id))
        {
            HttpContext.Session.Set(Keys.BudgetFile, id.ToByteArray());
            ViewBag.BudgetId = id; // If we use it as a model, it gets written to the page.
            return View();
        }

        return RedirectToAction("Index");
    }
}