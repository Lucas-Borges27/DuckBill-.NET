using System.Net;
using System.Net.Http.Json;
using DuckBill.Application.DTOs;
using Xunit;

namespace DuckBill.IntegrationTests;

[Collection("API collection")]
public class UsuariosCrudEndpointsTests
{
    private readonly ApiFactory _factory;

    public UsuariosCrudEndpointsTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetUsuarios_SemApiKey_RetornaUnauthorized()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
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
        await _factory.ResetDatabaseAsync();
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
    public async Task PostUsuario_DadosInvalidos_RetornaBadRequest()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "test-key");
        var dto = new UsuarioCreateDto(string.Empty, "ana@duckbill.com", "123");

        // Act
        var response = await client.PostAsJsonAsync("/api/usuarios", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostUsuario_EmailDuplicado_RetornaConflict()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "test-key");
        var dto = new UsuarioCreateDto("Ana duplicada", "ana@duckbill.com", "123");

        // Act
        var response = await client.PostAsJsonAsync("/api/usuarios", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task UsuarioEndpoints_FluxoCompleto_RetornaCreatedNoContentEOk()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "test-key");
        var createDto = new UsuarioCreateDto("Bruno", "bruno@duckbill.com", "123");
        var updateDto = new UsuarioCreateDto("Bruno Atualizado", "bruno@duckbill.com", "456");

        // Act
        var createResponse = await client.PostAsJsonAsync("/api/usuarios", createDto);
        var createdUsuario = await createResponse.Content.ReadFromJsonAsync<UsuarioDto>();
        var getCreatedResponse = await client.GetAsync($"/api/usuarios/{createdUsuario!.Id}");
        var updateResponse = await client.PutAsJsonAsync($"/api/usuarios/{createdUsuario.Id}", updateDto);
        var getUpdatedResponse = await client.GetAsync($"/api/usuarios/{createdUsuario.Id}");
        var deleteResponse = await client.DeleteAsync($"/api/usuarios/{createdUsuario.Id}");
        var getDeletedResponse = await client.GetAsync($"/api/usuarios/{createdUsuario.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createdUsuario);
        Assert.Equal(HttpStatusCode.OK, getCreatedResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getUpdatedResponse.StatusCode);

        var updatedUsuario = await getUpdatedResponse.Content.ReadFromJsonAsync<UsuarioDto>();
        Assert.NotNull(updatedUsuario);
        Assert.Equal("Bruno Atualizado", updatedUsuario!.Nome);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }
}
