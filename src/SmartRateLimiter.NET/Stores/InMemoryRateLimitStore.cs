using System.Collections.Concurrent;

namespace SmartRateLimiter.NET.Stores;

public class InMemoryRateLimitStore : IRateLimitStore
{
    private readonly ConcurrentDictionary<string, List<DateTime>> _sliding = new();
    private readonly ConcurrentDictionary<string, (int Count, DateTime Expiry)> _fixed = new();
    private readonly ConcurrentDictionary<string, (int Tokens, DateTime Last)> _bucket = new();

    public Task<bool> CheckSlidingWindowAsync(string key, int limit, int windowSeconds)
    {
        var now = DateTime.UtcNow;
        var windowStart = now.AddSeconds(-windowSeconds);

        var list = _sliding.GetOrAdd(key, _ => new List<DateTime>());

        lock (list)
        {
            list.RemoveAll(x => x <= windowStart);

            if (list.Count >= limit)
                return Task.FromResult(false);

            list.Add(now);
            return Task.FromResult(true);
        }
    }

    public Task<bool> CheckFixedWindowAsync(string key, int limit, int windowSeconds)
    {
        var now = DateTime.UtcNow;

        if (!_fixed.ContainsKey(key) || _fixed[key].Expiry < now)
        {
            _fixed[key] = (1, now.AddSeconds(windowSeconds));
            return Task.FromResult(true);
        }

        var entry = _fixed[key];

        if (entry.Count >= limit)
            return Task.FromResult(false);

        _fixed[key] = (entry.Count + 1, entry.Expiry);
        return Task.FromResult(true);
    }

    public Task<bool> CheckTokenBucketAsync(string key, int capacity, int refillRate, int intervalSeconds)
    {
        var now = DateTime.UtcNow;

        var bucket = _bucket.GetOrAdd(key, _ => (capacity, now));

        var elapsed = (now - bucket.Last).TotalSeconds;
        var refill = (int)(elapsed / intervalSeconds) * refillRate;

        var tokens = Math.Min(capacity, bucket.Tokens + refill);

        if (tokens <= 0)
            return Task.FromResult(false);

        _bucket[key] = (tokens - 1, now);
        return Task.FromResult(true);
    }
}
