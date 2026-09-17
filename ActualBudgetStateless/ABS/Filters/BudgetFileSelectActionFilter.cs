using ABS.Configuration;
using ABS.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using static ABS.Configuration.Constants.Session;

namespace ABS.Filters;

public class BudgetFileSelectActionFilter(EnvironmentVariables env) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is Controller and not HomeController and not SwitchBudgetController)
        {
            if (!context.HttpContext.Session.Keys.Contains(Keys.BudgetFile))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "SwitchBudget", action = "Index" })
                );
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        
    }
}