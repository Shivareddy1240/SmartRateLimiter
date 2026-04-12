using SmartRateLimiter.NET.Models;

public class RateLimitStrategyResolver
{
    private readonly IEnumerable<IRateLimitStrategy> _strategies;

    public RateLimitStrategyResolver(IEnumerable<IRateLimitStrategy> strategies)
    {
        _strategies = strategies;
    }

    public IRateLimitStrategy Resolve(RateLimitType type)
        => _strategies.First(s => s.Type == type);
}
