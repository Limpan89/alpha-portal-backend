namespace Presentation.Extensions.Middlewares;

public class DefaultApiKeyMiddleware(RequestDelegate next, IConfiguration config)
{
    private readonly RequestDelegate _next = next;
    private readonly IConfiguration _config = config;
    private const string API_KEY_HEADER_NAME = "X-API-KEY";

    public async Task InvokeAsync(HttpContext context)
    {
        var defaultApiKey = _config["SecretKeys:Default"] ?? null;

        if (string.IsNullOrEmpty(defaultApiKey) || !context.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var provideApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid api-key or api-key is missing.");
            return;
        }

        if (!string.Equals(provideApiKey, defaultApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid api-key.");
            return;
        }

        await _next(context);
    }
}
