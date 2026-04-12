using Microsoft.Extensions.Logging;
using SmartRateLimiter.NET.Stores;

namespace SmartRateLimiter.NET.Stores;

public class HybridRateLimitStore : IRateLimitStore
{
    private readonly InMemoryRateLimitStore _memory;
    private readonly RedisRateLimitStore _redis;
    private readonly ILogger<HybridRateLimitStore> _logger;

    public HybridRateLimitStore(
        InMemoryRateLimitStore memory,
        RedisRateLimitStore redis,
        ILogger<HybridRateLimitStore> logger)
    {
        _memory = memory;
        _redis = redis;
        _logger = logger;
    }

    // 🔥 Sliding Window
    public async Task<bool> CheckSlidingWindowAsync(string key, int limit, int windowSeconds)
    {
        // Step 1: Fast check (memory)
        var memoryAllowed = await _memory.CheckSlidingWindowAsync(key, limit, windowSeconds);

        if (!memoryAllowed)
            return false;

        // Step 2: Redis (source of truth)
        try
        {
            var redisAllowed = await _redis.CheckSlidingWindowAsync(key, limit, windowSeconds);

            return redisAllowed;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis failed, falling back to memory");
            return true; // fallback
        }
    }

    // 🔥 Fixed Window
    public async Task<bool> CheckFixedWindowAsync(string key, int limit, int windowSeconds)
    {
        var memoryAllowed = await _memory.CheckFixedWindowAsync(key, limit, windowSeconds);

        if (!memoryAllowed)
            return false;

        try
        {
            return await _redis.CheckFixedWindowAsync(key, limit, windowSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis failed, fallback to memory");
            return true;
        }
    }

    // 🔥 Token Bucket
    public async Task<bool> CheckTokenBucketAsync(string key, int capacity, int refillRate, int intervalSeconds)
    {
        var memoryAllowed = await _memory.CheckTokenBucketAsync(key, capacity, refillRate, intervalSeconds);

        if (!memoryAllowed)
            return false;

        try
        {
            return await _redis.CheckTokenBucketAsync(key, capacity, refillRate, intervalSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis failed, fallback to memory");
            return true;
        }
    }
}
