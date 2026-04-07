using DuckBill.Application.DTOs;
using DuckBill.Application.Services;
using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;
using Moq;
using Xunit;

namespace DuckBill.UnitTests.Application;

public class DespesaServiceTests : IClassFixture<ApplicationServiceFixture>
{
    private readonly ApplicationServiceFixture _fixture;

    public DespesaServiceTests(ApplicationServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_DadosValidos_CriaDespesaComCategoria()
    {
        // Arrange
        var despesaRepository = _fixture.CreateDespesaRepositoryMock();
        var usuarioRepository = _fixture.CreateUsuarioRepositoryMock();
        var categoriaRepository = _fixture.CreateCategoriaRepositoryMock();
        var dto = new DespesaCreateDto(1, 2, 99.9m, "BRL", new DateTime(2026, 3, 30), "Mercado");

        usuarioRepository.Setup(r => r.GetByIdAsync(dto.UsuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Id = dto.UsuarioId, Nome = "Ana", Email = "ana@duckbill.com", Senha = "123" });
        categoriaRepository.Setup(r => r.GetByIdAsync(dto.CategoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Categoria { Id = dto.CategoriaId, Nome = "Alimentacao" });
        despesaRepository.Setup(r => r.AddAsync(It.IsAny<Despesa>(), It.IsAny<CancellationToken>()))
            .Callback<Despesa, CancellationToken>((despesa, _) => despesa.Id = 25);

        var service = _fixture.CreateDespesaService(despesaRepository, usuarioRepository, categoriaRepository);

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
        var despesaRepository = _fixture.CreateDespesaRepositoryMock();
        var usuarioRepository = _fixture.CreateUsuarioRepositoryMock();
        var categoriaRepository = _fixture.CreateCategoriaRepositoryMock();
        var dto = new DespesaCreateDto(1, 2, 0m, "BRL", new DateTime(2026, 3, 30), "Mercado");

        var service = _fixture.CreateDespesaService(despesaRepository, usuarioRepository, categoriaRepository);

        // Act
        var act = () => service.CreateAsync(dto);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task CreateAsync_UsuarioInexistente_DisparaKeyNotFoundException()
    {
        // Arrange
        var despesaRepository = _fixture.CreateDespesaRepositoryMock();
        var usuarioRepository = _fixture.CreateUsuarioRepositoryMock();
        var categoriaRepository = _fixture.CreateCategoriaRepositoryMock();
        var dto = new DespesaCreateDto(999, 2, 50m, "BRL", new DateTime(2026, 3, 30), "Mercado");

        usuarioRepository.Setup(r => r.GetByIdAsync(dto.UsuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var service = _fixture.CreateDespesaService(despesaRepository, usuarioRepository, categoriaRepository);

        // Act
        var act = () => service.CreateAsync(dto);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(act);
    }

    [Fact]
    public async Task UpdateAsync_CategoriaInexistente_DisparaKeyNotFoundException()
    {
        // Arrange
        var despesaRepository = _fixture.CreateDespesaRepositoryMock();
        var usuarioRepository = _fixture.CreateUsuarioRepositoryMock();
        var categoriaRepository = _fixture.CreateCategoriaRepositoryMock();
        var dto = new DespesaCreateDto(1, 99, 50m, "BRL", new DateTime(2026, 3, 30), "Mercado");

        despesaRepository.Setup(r => r.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Despesa { Id = 7, UsuarioId = 1, CategoriaId = 2, Valor = 10m, Moeda = "BRL", DataCompra = new DateTime(2026, 3, 20) });
        usuarioRepository.Setup(r => r.GetByIdAsync(dto.UsuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario { Id = dto.UsuarioId, Nome = "Ana", Email = "ana@duckbill.com", Senha = "123" });
        categoriaRepository.Setup(r => r.GetByIdAsync(dto.CategoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Categoria?)null);

        var service = _fixture.CreateDespesaService(despesaRepository, usuarioRepository, categoriaRepository);

        // Act
        var act = () => service.UpdateAsync(7, dto);

        // Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(act);
    }

    [Fact]
    public async Task GetByUsuarioIdAsync_DespesasExistentes_RetornaCategoriaNomePreenchido()
    {
        // Arrange
        var despesaRepository = _fixture.CreateDespesaRepositoryMock();
        var usuarioRepository = _fixture.CreateUsuarioRepositoryMock();
        var categoriaRepository = _fixture.CreateCategoriaRepositoryMock();

        despesaRepository.Setup(r => r.GetByUsuarioIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Despesa>
            {
                new() { Id = 11, UsuarioId = 1, CategoriaId = 5, Valor = 150m, Moeda = "BRL", DataCompra = new DateTime(2026, 3, 1), Descricao = "Supermercado" }
            });
        categoriaRepository.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Categoria { Id = 5, Nome = "Alimentacao" });

        var service = _fixture.CreateDespesaService(despesaRepository, usuarioRepository, categoriaRepository);

        // Act
        var result = (await service.GetByUsuarioIdAsync(1)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Alimentacao", result[0].CategoriaNome);
        Assert.Equal("Supermercado", result[0].Descricao);
    }
}
