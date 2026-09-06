using ABS.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ABS.Filters;

public class ViewBagActionFilter(EnvironmentVariables env) : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is Controller controller)
        {
            controller.ViewBag.ActualLinkUrl = env.ActualLinkUrl;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        
    }
}