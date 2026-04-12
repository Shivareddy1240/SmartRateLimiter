public interface IRateLimitStore
{
    Task<bool> CheckSlidingWindowAsync(string key, int limit, int windowSeconds);
    Task<bool> CheckFixedWindowAsync(string key, int limit, int windowSeconds);
    Task<bool> CheckTokenBucketAsync(string key, int capacity, int refillRate, int intervalSeconds);
}
