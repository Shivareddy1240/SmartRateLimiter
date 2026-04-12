# 🚀 SmartRateLimiter.NET

A **plug & play, production-ready rate limiting library** for ASP.NET Core.

Supports multiple algorithms, storage strategies, and distributed systems.

---

## ✨ Features

* ⚡ Multiple algorithms:

  * Sliding Window (default)
  * Fixed Window
  * Token Bucket

* 🟢 In-Memory support (fast, simple)

* 🔴 Redis support (distributed systems)

* 🔥 Hybrid mode (Memory + Redis + fallback)

* 🎯 Attribute-based per-endpoint control

* 🧩 Middleware integration

* 📄 Swagger UI integration

* 🧠 Clean architecture (Strategy Pattern)

---

## 📦 Installation

```bash
dotnet add package SmartRateLimiter.NET
```

---

## ⚙️ Quick Start

### Program.cs

```csharp
builder.Services.AddSmartRateLimiter(options =>
{
    options.DefaultLimit = 5;
    options.DefaultWindowSeconds = 10;
});

app.UseSmartRateLimiter();
```

---

## 🔁 Storage Modes

---

### 🟢 In-Memory (Default)

```csharp
builder.Services.AddSmartRateLimiter();
```

✔ Fast
❌ Not shared across instances

---

### 🔴 Redis (Distributed)

```csharp
builder.Services.AddSmartRateLimiterWithRedis("localhost:6379");
```

✔ Shared across multiple instances
✔ Production ready

---

### 🔥 Hybrid (Recommended for Scale)

```csharp
builder.Services.AddSmartRateLimiterHybrid("localhost:6379");
```

✔ Fast (memory)
✔ Distributed (Redis)
✔ Resilient (fallback if Redis fails)

---

## 🎯 Usage (Per Endpoint)

```csharp
[RateLimit(Type = RateLimitType.SlidingWindow, Limit = 5, WindowSeconds = 10)]
[HttpGet("users")]
public IActionResult GetUsers()
{
    return Ok();
}
```

---

## 🔁 Algorithms

---

### Sliding Window

```csharp
[RateLimit(Type = RateLimitType.SlidingWindow, Limit = 5, WindowSeconds = 10)]
```

---

### Fixed Window

```csharp
[RateLimit(Type = RateLimitType.FixedWindow, Limit = 10, WindowSeconds = 60)]
```

---

### Token Bucket

```csharp
[RateLimit(Type = RateLimitType.TokenBucket, Capacity = 10, RefillRate = 2, IntervalSeconds = 5)]
```

---

## 📤 Response When Limit Exceeded

```http
HTTP 429 Too Many Requests
```

---

## 🧱 Architecture

```text
Client Request
      ↓
RateLimiter Middleware
      ↓
Strategy Resolver
      ↓
Algorithm Strategy
      ↓
Store (Memory / Redis / Hybrid)
      ↓
Allow / Reject
```

---

## 🧪 Demo Project

A working demo is available in `/demo` folder.

Run:

```bash
cd demo/SmartRateLimiter.Demo
dotnet run
```

Open Swagger:

```
https://localhost:xxxx/swagger
```

---

## ⚙️ Config-Based Mode Switching

```json
{
  "RateLimiter": {
    "Mode": "Hybrid",
    "RedisConnection": "localhost:6379"
  }
}
```

---

## 💡 Real Use Cases

* 🔐 Login APIs (strict rate limiting)
* 🛒 Product APIs (moderate limits)
* 💳 Payment APIs (smooth burst handling)
* 📡 Public APIs (abuse protection)

---

## 🛠 Tech Stack

* .NET 8
* ASP.NET Core Middleware
* Redis (StackExchange.Redis)
* Swagger (Swashbuckle)

---

## 📈 Roadmap

* [ ] Response headers (X-RateLimit-Remaining)
* [ ] Retry-After header
* [ ] Metrics & logging
* [ ] Dashboard UI
* [ ] API key-based rate limiting

---

## 🤝 Contributing

Pull requests welcome.

---

## 📜 License

MIT License
