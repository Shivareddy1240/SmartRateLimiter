using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SmartRateLimiter.NET.Models;
using SmartRateLimiter.NET.Stores;
using StackExchange.Redis;

public static class ServiceCollectionExtensions
{
    // 🟢 In-Memory
    public static IServiceCollection AddSmartRateLimiter(
        this IServiceCollection services,
        Action<RateLimitOptions>? configure = null)
    {
        var options = new RateLimitOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<IRateLimitStore, InMemoryRateLimitStore>();

        RegisterCore(services);
        return services;
    }

    // 🔴 Redis
    public static IServiceCollection AddSmartRateLimiterWithRedis(
        this IServiceCollection services,
        string redisConnection,
        Action<RateLimitOptions>? configure = null)
    {
        var options = new RateLimitOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConnection));

        services.AddSingleton<IRateLimitStore, RedisRateLimitStore>();

        RegisterCore(services);
        return services;
    }

    // 🔥 Hybrid
    public static IServiceCollection AddSmartRateLimiterHybrid(
        this IServiceCollection services,
        string redisConnection,
        Action<RateLimitOptions>? configure = null)
    {
        var options = new RateLimitOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConnection));

        services.AddSingleton<RedisRateLimitStore>();
        services.AddSingleton<InMemoryRateLimitStore>();

        services.AddSingleton<IRateLimitStore, HybridRateLimitStore>();

        RegisterCore(services);
        return services;
    }

    private static void RegisterCore(IServiceCollection services)
    {
        services.AddSingleton<IRateLimitStrategy, SlidingWindowStrategy>();
        services.AddSingleton<IRateLimitStrategy, FixedWindowStrategy>();
        services.AddSingleton<IRateLimitStrategy, TokenBucketStrategy>();

        services.AddSingleton<RateLimitStrategyResolver>();
    }

    public static IApplicationBuilder UseSmartRateLimiter(this IApplicationBuilder app)
        => app.UseMiddleware<RateLimiterMiddleware>();
}
