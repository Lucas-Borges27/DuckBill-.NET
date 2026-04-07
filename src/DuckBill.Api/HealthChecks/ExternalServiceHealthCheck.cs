using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DuckBill.Api.HealthChecks;

public sealed class ExternalServiceHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExternalServiceHealthCheck> _logger;

    public ExternalServiceHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ExternalServiceHealthCheck> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var baseUrl = _configuration["Integrations:AwesomeApi:BaseUrl"];
        var healthPath = _configuration["Integrations:AwesomeApi:HealthPath"] ?? "json/last/USD-BRL";
        var metadata = new Dictionary<string, object>
        {
            ["baseUrl"] = baseUrl ?? string.Empty,
            ["healthPath"] = healthPath
        };

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            _logger.LogError("External service health check failed because AwesomeApi BaseUrl is not configured");
            return HealthCheckResult.Unhealthy("AwesomeApi BaseUrl não configurada.", data: metadata);
        }

        try
        {
            var client = _httpClientFactory.CreateClient("awesomeapi");

            using var response = await client.GetAsync(healthPath, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("External service health check succeeded for {BaseUrl}{HealthPath}", baseUrl, healthPath);
                metadata["statusCode"] = (int)response.StatusCode;
                return HealthCheckResult.Healthy("AwesomeApi disponível.", metadata);
            }

            _logger.LogWarning("External service health check returned status code {StatusCode} for {BaseUrl}{HealthPath}", (int)response.StatusCode, baseUrl, healthPath);
            metadata["statusCode"] = (int)response.StatusCode;
            return HealthCheckResult.Unhealthy($"AwesomeApi retornou status {(int)response.StatusCode}.", data: metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External service health check failed while calling {BaseUrl}{HealthPath}", baseUrl, healthPath);
            return HealthCheckResult.Unhealthy("Falha ao acessar AwesomeApi.", ex, metadata);
        }
    }
}
