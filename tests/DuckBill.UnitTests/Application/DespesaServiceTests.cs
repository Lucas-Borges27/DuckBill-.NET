using DuckBill.Application.DTOs;
using DuckBill.Application.Services;
using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;
using Moq;
using Xunit;

namespace DuckBill.UnitTests.Application;

public class DespesaServiceTests
{
    [Fact]
    public async Task CreateAsync_DadosValidos_CriaDespesaComCategoria()
    {
        // Arrange
        var despesaRepository = new Mock<IDespesaRepository>();
        var usuarioRepository = new Mock<IUsuarioRepository>();
        var categoriaRepository = new Mock<ICategoriaRepository>();
        var dto = new DespesaCreateDto(1, 2, 99.9m, "BRL", new DateTime(2026, 3, 30), "Mercado");

        usuarioRepository.Setup(r => r.GetByIdAsync(dto.UsuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Id = dto.UsuarioId, Nome = "Ana", Email = "ana@duckbill.com", Senha = "123" });
        categoriaRepository.Setup(r => r.GetByIdAsync(dto.CategoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Categoria { Id = dto.CategoriaId, Nome = "Alimentacao" });
        despesaRepository.Setup(r => r.AddAsync(It.IsAny<Despesa>(), It.IsAny<CancellationToken>()))
            .Callback<Despesa, CancellationToken>((despesa, _) => despesa.Id = 25);

        var service = new DespesaService(despesaRepository.Object, usuarioRepository.Object, categoriaRepository.Object);

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Equal(25, result.Id);
        Assert.Equal(dto.UsuarioId, result.UsuarioId);
        Assert.Equal(dto.CategoriaId, result.CategoriaId);
        Assert.Equal("Alimentacao", result.CategoriaNome);
        despesaRepository.Verify(r => r.AddAsync(It.Is<Despesa>(d => d.UsuarioId == dto.UsuarioId && d.CategoriaId == dto.CategoriaId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ValorMenorOuIgualAZero_DisparaArgumentException()
    {
        // Arrange
        var despesaRepository = new Mock<IDespesaRepository>();
        var usuarioRepository = new Mock<IUsuarioRepository>();
        var categoriaRepository = new Mock<ICategoriaRepository>();
        var dto = new DespesaCreateDto(1, 2, 0m, "BRL", new DateTime(2026, 3, 30), "Mercado");

        var service = new DespesaService(despesaRepository.Object, usuarioRepository.Object, categoriaRepository.Object);

        // Act
        var act = () => service.CreateAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task CreateAsync_UsuarioInexistente_DisparaKeyNotFoundException()
    {
        // Arrange
        var despesaRepository = new Mock<IDespesaRepository>();
        var usuarioRepository = new Mock<IUsuarioRepository>();
        var categoriaRepository = new Mock<ICategoriaRepository>();
        var dto = new DespesaCreateDto(999, 2, 50m, "BRL", new DateTime(2026, 3, 30), "Mercado");

        usuarioRepository.Setup(r => r.GetByIdAsync(dto.UsuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var service = new DespesaService(despesaRepository.Object, usuarioRepository.Object, categoriaRepository.Object);

        // Act
        var act = () => service.CreateAsync(dto);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(act);
    }
}
