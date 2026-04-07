using DuckBill.Application.Services;
using DuckBill.Domain.Interfaces;
using Moq;

namespace DuckBill.UnitTests.Application;

public sealed class ApplicationServiceFixture
{
    public Mock<IUsuarioRepository> CreateUsuarioRepositoryMock() => new();

    public Mock<IDespesaRepository> CreateDespesaRepositoryMock() => new();

    public Mock<ICategoriaRepository> CreateCategoriaRepositoryMock() => new();

    public UsuarioService CreateUsuarioService(Mock<IUsuarioRepository> usuarioRepository) =>
        new(usuarioRepository.Object);

    public DespesaService CreateDespesaService(
        Mock<IDespesaRepository> despesaRepository,
        Mock<IUsuarioRepository> usuarioRepository,
        Mock<ICategoriaRepository> categoriaRepository) =>
        new(despesaRepository.Object, usuarioRepository.Object, categoriaRepository.Object);
}
