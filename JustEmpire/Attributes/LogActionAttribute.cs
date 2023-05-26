using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace JustEmpire.Attributes;

public class LogActionAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        string ipAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
        string url = request.Path;

        Log.Information($"Accessed {url} from {ipAddress}");

        base.OnActionExecuting(context);
    }
}