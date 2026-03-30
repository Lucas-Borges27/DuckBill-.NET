using DuckBill.Domain.Entities;
using Xunit;

namespace DuckBill.UnitTests.Domain;

public class DespesaTests
{
    [Fact]
    public void Despesa_ConstrucaoPadrao_DefineMoedaComoBRL()
    {
        // Arrange
        var despesa = new Despesa();

        // Act
        var moeda = despesa.Moeda;

        // Assert
        Assert.Equal("BRL", moeda);
    }
}
