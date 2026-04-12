var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// 🔥 Read config
var config = builder.Configuration.GetSection("RateLimiter");

var mode = config["Mode"];
var redisConnection = config["RedisConnection"];

var limit = config.GetValue<int>("DefaultLimit");
var window = config.GetValue<int>("DefaultWindowSeconds");

// 🔥 Switch between modes
switch (mode?.ToLower())
{
    case "redis":
        builder.Services.AddSmartRateLimiterWithRedis(redisConnection!, options =>
        {
            options.DefaultLimit = limit;
            options.DefaultWindowSeconds = window;
        });
        break;

    case "hybrid":
        builder.Services.AddSmartRateLimiterHybrid(redisConnection!, options =>
        {
            options.DefaultLimit = limit;
            options.DefaultWindowSeconds = window;
        });
        break;

    default:
        builder.Services.AddSmartRateLimiter(options =>
        {
            options.DefaultLimit = limit;
            options.DefaultWindowSeconds = window;
        });
        break;
}

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<RateLimitOperationFilter>();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// 🔥 Rate Limiter
app.UseSmartRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
