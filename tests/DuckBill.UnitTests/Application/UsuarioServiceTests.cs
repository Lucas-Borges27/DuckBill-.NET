using DuckBill.Application.DTOs;
using DuckBill.Application.Services;
using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;
using Moq;
using Xunit;

namespace DuckBill.UnitTests.Application;

public class UsuarioServiceTests
{
    [Fact]
    public async Task CreateAsync_DadosValidos_CriaUsuario()
    {
        // Arrange
        var repo = new Mock<IUsuarioRepository>();
        var dto = new UsuarioCreateDto("Ana", "ana@duckbill.com", "123");

        repo.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        repo.Setup(r => r.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .Callback<Usuario, CancellationToken>((u, _) => u.Id = 10);

        var service = new UsuarioService(repo.Object);

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
        var repo = new Mock<IUsuarioRepository>();
        var dto = new UsuarioCreateDto("Ana", "ana@duckbill.com", "123");

        repo.Setup(r => r.GetByEmailAsync(dto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Id = 1, Nome = "Outra", Email = dto.Email, Senha = "x" });

        var service = new UsuarioService(repo.Object);

        // Act
        var act = () => service.CreateAsync(dto);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task UpdateAsync_UsuarioInexistente_DisparaKeyNotFoundException()
    {
        // Arrange
        var repo = new Mock<IUsuarioRepository>();
        var dto = new UsuarioCreateDto("Ana", "ana@duckbill.com", "123");

        repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var service = new UsuarioService(repo.Object);

        // Act
        var act = () => service.UpdateAsync(99, dto);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(act);
    }
}
