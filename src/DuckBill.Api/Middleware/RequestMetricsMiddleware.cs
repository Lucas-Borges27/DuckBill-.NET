using System.Diagnostics;
using DuckBill.Application.Observability;

namespace DuckBill.Api.Middleware;

public sealed class RequestMetricsMiddleware
{
    private readonly RequestDelegate _next;

    public RequestMetricsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var tags = new TagList
            {
                { "http.request.method", context.Request.Method },
                { "url.path", context.Request.Path.Value ?? string.Empty },
                { "http.response.status_code", context.Response.StatusCode }
            };

            ApplicationMetrics.RequestCount.Add(1, tags);
            ApplicationMetrics.RequestDurationMs.Record(stopwatch.Elapsed.TotalMilliseconds, tags);

            if (context.Response.StatusCode >= StatusCodes.Status400BadRequest)
            {
                ApplicationMetrics.RequestErrorCount.Add(1, tags);
            }
        }
    }
}
