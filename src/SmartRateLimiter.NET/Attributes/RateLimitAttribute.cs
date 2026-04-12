using SmartRateLimiter.NET.Models;

public class RateLimitAttribute : Attribute
{
    public RateLimitType Type { get; set; } = RateLimitType.SlidingWindow;

    public int Limit { get; set; } = 10;
    public int WindowSeconds { get; set; } = 60;

    public int Capacity { get; set; } = 10;
    public int RefillRate { get; set; } = 1;
    public int IntervalSeconds { get; set; } = 1;
}
