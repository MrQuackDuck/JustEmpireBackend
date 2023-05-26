using JustEmpire.Models.Classes;
using JustEmpire.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace JustEmpire.Attributes;

public class CountViewAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var pageViewRepository = context.HttpContext.RequestServices.GetService<PageViewRepository>();

        PageView pageView = new PageView()
        {
            Date = DateTime.Now
        };
        pageViewRepository.Create(pageView);
        
        base.OnActionExecuting(context);
    }
}