using DuckBill.Domain.Entities;

namespace DuckBill.Domain.Interfaces;

public interface IAtivoRepository
{
    Task<Ativo?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Ativo?> GetByTickerAsync(string ticker, CancellationToken ct = default);
    Task<IEnumerable<Ativo>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Ativo ativo, CancellationToken ct = default);
    Task UpdateAsync(Ativo ativo, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
