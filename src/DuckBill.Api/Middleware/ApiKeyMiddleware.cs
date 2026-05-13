namespace DuckBill.Api.Middleware;

public sealed class ApiKeyMiddleware
{
    private const string HeaderName = "X-API-KEY";
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/health", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/metrics", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var expectedApiKey = Environment.GetEnvironmentVariable("API_KEY")
            ?? _configuration["Authentication:ApiKey"];
        if (string.IsNullOrWhiteSpace(expectedApiKey))
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<ApiKeyMiddleware>>();
            logger.LogError("Authentication is enabled but the API key is not configured");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "API key não configurada." });
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderName, out var provided) || provided != expectedApiKey)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<ApiKeyMiddleware>>();
            logger.LogWarning("Unauthorized request for {Path}: API key missing or invalid", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "API key inválida ou ausente." });
            return;
        }

        await _next(context);
    }
}
