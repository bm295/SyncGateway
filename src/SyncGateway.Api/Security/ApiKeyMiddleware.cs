using Microsoft.Extensions.Options;

namespace SyncGateway.Api.Security;

public sealed class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly GatewayOptions _options;

    public ApiKeyMiddleware(RequestDelegate next, IOptions<GatewayOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var apiKey)
            || apiKey != _options.ApiKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "unauthorized",
                message = "Missing or invalid X-Api-Key header."
            });
            return;
        }

        await _next(context);
    }
}
