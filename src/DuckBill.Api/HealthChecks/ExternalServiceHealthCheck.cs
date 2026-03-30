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

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            _logger.LogError("External service health check failed because AwesomeApi BaseUrl is not configured");
            return HealthCheckResult.Unhealthy("AwesomeApi BaseUrl não configurada.");
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);

            using var response = await client.GetAsync(healthPath, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("External service health check succeeded for {BaseUrl}{HealthPath}", baseUrl, healthPath);
                return HealthCheckResult.Healthy("AwesomeApi disponível.");
            }

            _logger.LogWarning("External service health check returned status code {StatusCode} for {BaseUrl}{HealthPath}", (int)response.StatusCode, baseUrl, healthPath);
            return HealthCheckResult.Unhealthy($"AwesomeApi retornou status {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "External service health check failed while calling {BaseUrl}{HealthPath}", baseUrl, healthPath);
            return HealthCheckResult.Unhealthy("Falha ao acessar AwesomeApi.", ex);
        }
    }
}
