using DuckBill.Domain.Entities;

namespace DuckBill.Domain.Interfaces;

public interface ITransacaoAtivoRepository
{
    Task<TransacaoAtivo?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IEnumerable<TransacaoAtivo>> GetByUsuarioIdAsync(long usuarioId, CancellationToken ct = default);
    Task AddAsync(TransacaoAtivo transacao, CancellationToken ct = default);
    Task UpdateAsync(TransacaoAtivo transacao, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
