using DuckBill.Domain.Entities;

namespace DuckBill.Domain.Interfaces;

public interface ICotacaoAtivoRepository
{
    Task<CotacaoAtivo?> GetByAtivoIdAndDataAsync(long ativoId, DateTime dataRef, CancellationToken ct = default);
    Task<IEnumerable<CotacaoAtivo>> GetByAtivoIdAsync(long ativoId, CancellationToken ct = default);
    Task AddAsync(CotacaoAtivo cotacao, CancellationToken ct = default);
    Task UpdateAsync(CotacaoAtivo cotacao, CancellationToken ct = default);
    Task DeleteAsync(long ativoId, DateTime dataRef, CancellationToken ct = default);
}
