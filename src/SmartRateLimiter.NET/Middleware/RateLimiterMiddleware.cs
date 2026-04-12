using Microsoft.AspNetCore.Http;
using SmartRateLimiter.NET.Models;

public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimitOptions _options;

    public RateLimiterMiddleware(RequestDelegate next, RateLimitOptions options)
    {
        _next = next;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context, RateLimitStrategyResolver resolver)
    {
        var endpoint = context.GetEndpoint();

        var attr = endpoint?.Metadata.GetMetadata<RateLimitAttribute>()
            ?? new RateLimitAttribute
            {
                Limit = _options.DefaultLimit,
                WindowSeconds = _options.DefaultWindowSeconds
            };

        var strategy = resolver.Resolve(attr.Type);

        var key = $"{context.Connection.RemoteIpAddress}:{endpoint?.DisplayName}";

        if (!await strategy.IsAllowedAsync(key, attr))
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsync("Too many requests");
            return;
        }

        await _next(context);
    }
}
