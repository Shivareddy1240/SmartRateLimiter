using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

public class RateLimitOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attr = context.MethodInfo.GetCustomAttributes(true)
            .OfType<RateLimitAttribute>()
            .FirstOrDefault();

        if (attr != null)
        {
            operation.Description +=
                $"\n\n🔒 Rate Limit: {attr.Type} | {attr.Limit}/{attr.WindowSeconds}s";
        }
    }
}
