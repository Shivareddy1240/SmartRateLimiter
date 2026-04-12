using StackExchange.Redis;

namespace SmartRateLimiter.NET.Stores;

public class RedisRateLimitStore : IRateLimitStore
{
    private readonly IDatabase _db;

    public RedisRateLimitStore(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<bool> CheckSlidingWindowAsync(string key, int limit, int windowSeconds)
    {
        var redisKey = $"sw:{key}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var windowStart = now - windowSeconds;

        await _db.SortedSetRemoveRangeByScoreAsync(redisKey, 0, windowStart);

        var count = await _db.SortedSetLengthAsync(redisKey);

        if (count >= limit)
            return false;

        await _db.SortedSetAddAsync(redisKey, Guid.NewGuid().ToString(), now);
        await _db.KeyExpireAsync(redisKey, TimeSpan.FromSeconds(windowSeconds));

        return true;
    }

    public async Task<bool> CheckFixedWindowAsync(string key, int limit, int windowSeconds)
    {
        var redisKey = $"fw:{key}";
        var count = await _db.StringIncrementAsync(redisKey);

        if (count == 1)
            await _db.KeyExpireAsync(redisKey, TimeSpan.FromSeconds(windowSeconds));

        return count <= limit;
    }

    public async Task<bool> CheckTokenBucketAsync(string key, int capacity, int refillRate, int intervalSeconds)
    {
        var redisKey = $"tb:{key}";
        var tokens = await _db.StringIncrementAsync(redisKey);

        if (tokens == 1)
            await _db.KeyExpireAsync(redisKey, TimeSpan.FromSeconds(intervalSeconds));

        return tokens <= capacity;
    }
}
