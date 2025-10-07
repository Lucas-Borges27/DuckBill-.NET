using DuckBill.Domain.Entities;

namespace DuckBill.Domain.Interfaces;

public interface ICategoriaRepository
{
    Task<Categoria?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Categoria?> GetByNomeAsync(string nome, CancellationToken ct = default);
    Task<IEnumerable<Categoria>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Categoria categoria, CancellationToken ct = default);
    Task UpdateAsync(Categoria categoria, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
