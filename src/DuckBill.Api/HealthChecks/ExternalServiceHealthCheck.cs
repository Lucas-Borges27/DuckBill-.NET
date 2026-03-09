using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DuckBill.Api.HealthChecks;

public sealed class ExternalServiceHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public ExternalServiceHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var baseUrl = _configuration["Integrations:AwesomeApi:BaseUrl"];
        var healthPath = _configuration["Integrations:AwesomeApi:HealthPath"] ?? "json/last/USD-BRL";

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return HealthCheckResult.Unhealthy("AwesomeApi BaseUrl não configurada.");
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);

            using var response = await client.GetAsync(healthPath, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("AwesomeApi disponível.");
            }

            return HealthCheckResult.Unhealthy($"AwesomeApi retornou status {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Falha ao acessar AwesomeApi.", ex);
        }
    }
}
