namespace SmartRateLimiter.NET.Models;

public class RateLimitOptions
{
    public int DefaultLimit { get; set; } = 10;
    public int DefaultWindowSeconds { get; set; } = 60;
}
