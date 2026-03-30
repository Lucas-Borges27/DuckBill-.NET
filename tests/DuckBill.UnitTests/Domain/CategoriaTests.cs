using DuckBill.Domain.Entities;
using Xunit;

namespace DuckBill.UnitTests.Domain;

public class CategoriaTests
{
    [Fact]
    public void Categoria_ConstrucaoPadrao_InicializaColecaoDespesas()
    {
        // Arrange
        var categoria = new Categoria();

        // Act
        var despesas = categoria.Despesas;

        // Assert
        Assert.NotNull(despesas);
        Assert.Empty(despesas);
    }
}
