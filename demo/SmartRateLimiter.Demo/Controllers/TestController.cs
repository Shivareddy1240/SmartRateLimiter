using Microsoft.AspNetCore.Mvc;
using SmartRateLimiter.NET.Models;

namespace SmartRateLimiter.Demo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    // 🔥 Sliding Window
    [HttpGet("sliding")]
    [RateLimit(Type = RateLimitType.SlidingWindow, Limit = 5, WindowSeconds = 10)]
    public IActionResult Sliding()
    {
        return Ok("Sliding window: 5 requests per 10 seconds");
    }

    // 🔥 Fixed Window
    [HttpGet("fixed")]
    [RateLimit(Type = RateLimitType.FixedWindow, Limit = 3, WindowSeconds = 10)]
    public IActionResult Fixed()
    {
        return Ok("Fixed window: 3 requests per 10 seconds");
    }

    // 🔥 Token Bucket
    [HttpGet("token")]
    [RateLimit(Type = RateLimitType.TokenBucket, Capacity = 5, RefillRate = 1, IntervalSeconds = 5)]
    public IActionResult Token()
    {
        return Ok("Token bucket: capacity 5, refill 1 token every 5 sec");
    }

    // 🔥 Default (No Attribute)
    [HttpGet("default")]
    public IActionResult Default()
    {
        return Ok("Default limiter applied (5 requests per 10 seconds)");
    }
    [HttpGet("burst")]
    [RateLimit(Type = RateLimitType.TokenBucket, Capacity = 10, RefillRate = 2, IntervalSeconds = 5)]
    public IActionResult Burst()
    {
        return Ok("Burst traffic handled via Token Bucket");
    }
}
