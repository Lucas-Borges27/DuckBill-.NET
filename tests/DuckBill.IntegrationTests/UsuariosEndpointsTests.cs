using System.Net;
using System.Net.Http.Json;
using DuckBill.Application.DTOs;
using Xunit;

namespace DuckBill.IntegrationTests;

public class UsuariosEndpointsTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public UsuariosEndpointsTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetUsuarios_SemApiKey_RetornaUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/usuarios");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUsuarios_ComApiKey_RetornaSucesso()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "test-key");

        // Act
        var response = await client.GetAsync("/api/usuarios");

        // Assert
        response.EnsureSuccessStatusCode();
        var usuarios = await response.Content.ReadFromJsonAsync<List<UsuarioDto>>();
        Assert.NotNull(usuarios);
        Assert.NotEmpty(usuarios!);
    }

    [Fact]
    public async Task GetUsuarioInexistente_ComApiKey_RetornaNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "test-key");

        // Act
        var response = await client.GetAsync("/api/usuarios/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
