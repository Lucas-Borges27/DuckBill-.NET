using DuckBill.Domain.Entities;
using Xunit;

namespace DuckBill.UnitTests.Domain;

public class UsuarioTests
{
    [Fact]
    public void Usuario_AtribuirPropriedades_PersistemValores()
    {
        // Arrange
        var usuario = new Usuario();

        // Act
        usuario.Id = 1;
        usuario.Nome = "Ana";
        usuario.Email = "ana@duckbill.com";
        usuario.Senha = "123";

        // Assert
        Assert.Equal(1, usuario.Id);
        Assert.Equal("Ana", usuario.Nome);
        Assert.Equal("ana@duckbill.com", usuario.Email);
        Assert.Equal("123", usuario.Senha);
    }
}
