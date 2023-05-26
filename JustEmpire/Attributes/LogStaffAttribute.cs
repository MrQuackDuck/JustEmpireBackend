using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace JustEmpire.Attributes;

public class LogStaffAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        var user = context.HttpContext.User.Identity.Name;
        string ipAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
        string url = request.Path;

        Log.Information($"{user} accessed {url} from {ipAddress}");

        base.OnActionExecuting(context);
    }
}