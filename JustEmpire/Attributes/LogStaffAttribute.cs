using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace JustEmpire.Attributes;

/// <summary>
/// Log action for authorized user
/// </summary>
public class LogStaffAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        var user = context.HttpContext.User.Identity!.Name;
        string ipAddress = context.HttpContext.Connection.RemoteIpAddress!.ToString();
        string url = request.Path;

        Log.Information("{User} accessed {Url} from {IpAddress}", user, url, ipAddress);

        base.OnActionExecuting(context);
    }
}