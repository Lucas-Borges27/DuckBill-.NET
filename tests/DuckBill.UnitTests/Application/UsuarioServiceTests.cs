using DuckBill.Application.DTOs;
using DuckBill.Application.Services;
using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;
using Moq;
using Xunit;

namespace DuckBill.UnitTests.Application;

public class UsuarioServiceTests : IClassFixture<ApplicationServiceFixture>
{
    private readonly ApplicationServiceFixture _fixture;

    public UsuarioServiceTests(ApplicationServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_DadosValidos_CriaUsuario()
    {
        // Arrange
        var repo = _fixture.CreateUsuarioRepositoryMock();
        var dto = new UsuarioCreateDto("Ana", "ana@duckbill.com", "123");

        repo.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        repo.Setup(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .Callback<Usuario, CancellationToken>((u, _) => u.Id = 10);

        var service = _fixture.CreateUsuarioService(repo);

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Equal(10, result.Id);
        Assert.Equal(dto.Nome, result.Nome);
        Assert.Equal(dto.Email, result.Email);
        repo.Verify(r => r.AddAsync(It.Is<Usuario>(u => u.Email == dto.Email), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_EmailDuplicado_DisparaInvalidOperationException()
    {
        // Arrange
        var repo = _fixture.CreateUsuarioRepositoryMock();
        var dto = new UsuarioCreateDto("Ana", "ana@duckbill.com", "123");

        repo.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Id = 1, Nome = "Outra", Email = dto.Email, Senha = "x" });

        var service = _fixture.CreateUsuarioService(repo);

        // Act
        var act = () => service.CreateAsync(dto);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task UpdateAsync_UsuarioInexistente_DisparaKeyNotFoundException()
    {
        // Arrange
        var repo = _fixture.CreateUsuarioRepositoryMock();
        var dto = new UsuarioCreateDto("Ana", "ana@duckbill.com", "123");

        repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var service = _fixture.CreateUsuarioService(repo);

        // Act
        var act = () => service.UpdateAsync(99, dto);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(act);
    }

    [Fact]
    public async Task UpdateAsync_EmailDeOutroUsuario_DisparaInvalidOperationException()
    {
        // Arrange
        var repo = _fixture.CreateUsuarioRepositoryMock();
        var dto = new UsuarioCreateDto("Ana Atualizada", "ana@duckbill.com", "456");

        repo.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Id = 10, Nome = "Ana", Email = "ana-antiga@duckbill.com", Senha = "123" });
        repo.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Id = 20, Nome = "Outra", Email = dto.Email, Senha = "xyz" });

        var service = _fixture.CreateUsuarioService(repo);

        // Act
        var act = () => service.UpdateAsync(10, dto);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task SearchAsync_PaginacaoComFiltroEOrdenacao_RetornaLinksEMetadadosConsistentes()
    {
        // Arrange
        var repo = _fixture.CreateUsuarioRepositoryMock();
        var usuarios = new List<Usuario>
        {
            new() { Id = 1, Nome = "Bruno", Email = "bruno@duckbill.com", Senha = "123" },
            new() { Id = 2, Nome = "Ana", Email = "ana@duckbill.com", Senha = "123" },
            new() { Id = 3, Nome = "Carlos", Email = "carlos@duckbill.com", Senha = "123" }
        };

        repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuarios);

        var service = _fixture.CreateUsuarioService(repo);

        // Act
        var result = await service.SearchAsync("duckbill", "nome,asc", page: 2, size: 1);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("Bruno", result.Items.Single().Nome);
        Assert.Equal(2, result.Page);
        Assert.Equal(3, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal("/api/usuarios/search?filter=duckbill&sort=nome,asc&page=2&size=1", result._links["self"]);
        Assert.Equal("/api/usuarios/search?filter=duckbill&sort=nome,asc&page=1&size=1", result._links["prev"]);
        Assert.Equal("/api/usuarios/search?filter=duckbill&sort=nome,asc&page=3&size=1", result._links["next"]);
    }
}
