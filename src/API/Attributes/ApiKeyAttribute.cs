using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SimpleLibrary.API.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    private readonly string apiKeyHeader = "ApiKey";
    private readonly string[] apiKeys;

    public ApiKeyAttribute(params string[] apiKeys)
    {
        this.apiKeys = apiKeys;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if(!context.HttpContext.Request.Headers.TryGetValue(apiKeyHeader, out var apiKeyFromRequest))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        List<string> apiKeyValues = new();
        foreach(string apiKeyName in apiKeys)
        {
            var temp = config.GetValue<string>($"ApiKeys:{apiKeyName}") ?? "";
            apiKeyValues.Add(temp);
        } 

        if(!apiKeyValues.Any(key => key == apiKeyFromRequest))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await next();
    }
}
