using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentation.Extensions.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class UseAdminApiKeyAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var adminApiKey = config["SecretKeys:Admin"];

        if (!context.HttpContext.Request.Headers.TryGetValue("X-ADM-API-KEY", out var provideApiKey))
        {
            context.Result = new UnauthorizedObjectResult("Invalid api-key or api-key is missing.");
            return;
        }

        if (string.IsNullOrEmpty(adminApiKey) || !string.Equals(provideApiKey, adminApiKey))
        {
            context.Result = new UnauthorizedObjectResult("Invalid api-key.");
            return;
        }

        await next();
    }
}
