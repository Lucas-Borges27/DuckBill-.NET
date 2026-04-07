using System.Net;
using Xunit;

namespace DuckBill.IntegrationTests;

[Collection("API collection")]
public class ObservabilityEndpointsTests
{
    private readonly ApiFactory _factory;

    public ObservabilityEndpointsTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthLive_SemApiKey_RetornaHealthy()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Healthy", body);
        Assert.Contains("self", body);
    }

    [Fact]
    public async Task HealthReady_SemApiKey_RetornaHealthy()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/ready");
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("oracle-db", body);
        Assert.Contains("awesomeapi", body);
        Assert.Contains("baseUrl", body);
        Assert.Contains("healthPath", body);
    }

    [Fact]
    public async Task Metrics_AposRequisicoes_ExpoeTempoDeRespostaETaxaDeErros()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "test-key");

        // Act
        await client.GetAsync("/api/usuarios");
        client.DefaultRequestHeaders.Remove("X-API-KEY");
        await client.GetAsync("/api/usuarios");
        var metricsResponse = await client.GetAsync("/metrics");
        var metricsBody = await metricsResponse.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, metricsResponse.StatusCode);
        Assert.Contains("duckbill_http_server_request_duration_ms", metricsBody);
        Assert.Contains("duckbill_http_server_request_errors", metricsBody);
        Assert.Contains("duckbill_http_server_requests", metricsBody);
    }

    [Fact]
    public async Task GetUsuarios_ComCorrelationId_ReplicaHeaderNaResposta()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "test-key");
        client.DefaultRequestHeaders.Add("X-Correlation-ID", "video-sprint3");

        // Act
        var response = await client.GetAsync("/api/usuarios");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("X-Correlation-ID", out var values));
        Assert.Contains("video-sprint3", values);
    }
}
