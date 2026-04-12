using SmartRateLimiter.NET.Models;

public class SlidingWindowStrategy : IRateLimitStrategy
{
    public RateLimitType Type => RateLimitType.SlidingWindow;
    private readonly IRateLimitStore _store;

    public SlidingWindowStrategy(IRateLimitStore store) => _store = store;

    public Task<bool> IsAllowedAsync(string key, RateLimitAttribute config)
        => _store.CheckSlidingWindowAsync(key, config.Limit, config.WindowSeconds);
}
