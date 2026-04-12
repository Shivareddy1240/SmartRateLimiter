using SmartRateLimiter.NET.Models;

public class TokenBucketStrategy : IRateLimitStrategy
{
    public RateLimitType Type => RateLimitType.TokenBucket;
    private readonly IRateLimitStore _store;

    public TokenBucketStrategy(IRateLimitStore store) => _store = store;

    public Task<bool> IsAllowedAsync(string key, RateLimitAttribute config)
        => _store.CheckTokenBucketAsync(key, config.Capacity, config.RefillRate, config.IntervalSeconds);
}
