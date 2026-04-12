using SmartRateLimiter.NET.Models;

public class FixedWindowStrategy : IRateLimitStrategy
{
    public RateLimitType Type => RateLimitType.FixedWindow;
    private readonly IRateLimitStore _store;

    public FixedWindowStrategy(IRateLimitStore store) => _store = store;

    public Task<bool> IsAllowedAsync(string key, RateLimitAttribute config)
        => _store.CheckFixedWindowAsync(key, config.Limit, config.WindowSeconds);
}
