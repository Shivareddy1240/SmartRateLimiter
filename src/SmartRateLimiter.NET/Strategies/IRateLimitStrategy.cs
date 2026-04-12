using SmartRateLimiter.NET.Models;

public interface IRateLimitStrategy
{
    RateLimitType Type { get; }
    Task<bool> IsAllowedAsync(string key, RateLimitAttribute config);
}
