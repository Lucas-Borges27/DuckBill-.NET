using DuckBill.Api.Endpoints;
using DuckBill.Api.HealthChecks;
using DuckBill.Api.Middleware;
using DuckBill.Application.Observability;
using DuckBill.Domain.Interfaces;
using DuckBill.Application.Services;
using DuckBill.Infrastructure;
using DuckBill.Infrastructure.Repositories;
using DuckBill.Infrastructure.External;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, services, logger) =>
{
    logger.MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "DuckBill.Api")
        .WriteTo.Console(new CompactJsonFormatter())
        .WriteTo.File(new CompactJsonFormatter(), "logs/log-.json", rollingInterval: RollingInterval.Day);
});

// Oracle DB
builder.Services.AddDbContext<DuckBillDbContext>(opt =>
{
    opt.UseOracle(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IDespesaRepository, DespesaRepository>();
builder.Services.AddScoped<IAtivoRepository, AtivoRepository>();
builder.Services.AddScoped<ITransacaoAtivoRepository, TransacaoAtivoRepository>();
builder.Services.AddScoped<ICotacaoAtivoRepository, CotacaoAtivoRepository>();

// Services
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<DespesaService>();
builder.Services.AddScoped<AtivoService>();
builder.Services.AddScoped<TransacaoAtivoService>();
builder.Services.AddScoped<RelatoriosService>();

builder.Services.AddHttpClient("awesomeapi", c =>
{
    c.BaseAddress = new Uri(builder.Configuration["Integrations:AwesomeApi:BaseUrl"] ?? "https://economia.awesomeapi.com.br/");
});

builder.Services.AddHttpClient<ICambioService, AwesomeApiCambioService>("awesomeapi");

builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API saudável"), tags: new[] { "live" })
    .AddDbContextCheck<DuckBillDbContext>("oracle-db", tags: new[] { "ready" })
    .AddCheck<ExternalServiceHealthCheck>("awesomeapi", tags: new[] { "ready" });

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: "DuckBill.Api", serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource(Telemetry.ActivitySourceName)
        .AddConsoleExporter())
    .WithMetrics(metrics => metrics
        .AddMeter(ApplicationMetrics.MeterName)
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter());

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestMetricsMiddleware>();

// Global exception handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred.",
            details = ex.Message,
            correlationId = context.Items["X-Correlation-ID"]?.ToString(),
            traceId = Activity.Current?.TraceId.ToString()
        });
    }
});

if (builder.Configuration.GetValue<bool>("Authentication:Enabled"))
{
    app.UseMiddleware<ApiKeyMiddleware>();
}

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, _, exception) =>
    {
        if (exception is not null || httpContext.Response.StatusCode >= StatusCodes.Status500InternalServerError)
        {
            return LogEventLevel.Error;
        }

        if (httpContext.Response.StatusCode >= StatusCodes.Status400BadRequest)
        {
            return LogEventLevel.Warning;
        }

        return LogEventLevel.Information;
    };
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("CorrelationId", httpContext.Items["X-Correlation-ID"]?.ToString());
        diagnosticContext.Set("TraceId", Activity.Current?.TraceId.ToString());
        diagnosticContext.Set("SpanId", Activity.Current?.SpanId.ToString());
        diagnosticContext.Set("RequestPath", httpContext.Request.Path.ToString());
    };
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/swagger")
    {
        context.Response.Redirect("/swagger/index.html");
        return;
    }
    await next();
});

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DuckBill API"));

app.MapCategorias().MapDespesas().MapAtivos().MapTransacoesAtivo().MapRelatorios().MapUsuarios();

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

app.MapPrometheusScrapingEndpoint("/metrics");

app.Logger.LogInformation("DuckBill API started with health endpoints at /health/live and /health/ready, and metrics at /metrics");

app.Run();

public partial class Program { }
