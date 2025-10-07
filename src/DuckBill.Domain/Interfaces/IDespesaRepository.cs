using DuckBill.Domain.Entities;

namespace DuckBill.Domain.Interfaces;

public interface IDespesaRepository
{
    Task<Despesa?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IEnumerable<Despesa>> GetByUsuarioIdAsync(long usuarioId, CancellationToken ct = default);
    Task<IEnumerable<Despesa>> GetByUsuarioIdAndMesAnoAsync(long usuarioId, int mes, int ano, CancellationToken ct = default);
    Task AddAsync(Despesa despesa, CancellationToken ct = default);
    Task UpdateAsync(Despesa despesa, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
