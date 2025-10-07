using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DuckBill.Infrastructure.Repositories;

public class CotacaoAtivoRepository : ICotacaoAtivoRepository
{
    private readonly DuckBillDbContext _db;

    public CotacaoAtivoRepository(DuckBillDbContext db)
    {
        _db = db;
    }

    public async Task<CotacaoAtivo?> GetByAtivoIdAndDataAsync(long ativoId, DateTime dataRef, CancellationToken ct = default)
    {
        return await _db.CotacoesAtivo.FirstOrDefaultAsync(c => c.AtivoId == ativoId && c.DataRef == dataRef, ct);
    }

    public async Task<IEnumerable<CotacaoAtivo>> GetByAtivoIdAsync(long ativoId, CancellationToken ct = default)
    {
        return await _db.CotacoesAtivo.Where(c => c.AtivoId == ativoId).ToListAsync(ct);
    }

    public async Task AddAsync(CotacaoAtivo cotacao, CancellationToken ct = default)
    {
        await _db.CotacoesAtivo.AddAsync(cotacao, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(CotacaoAtivo cotacao, CancellationToken ct = default)
    {
        _db.CotacoesAtivo.Update(cotacao);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long ativoId, DateTime dataRef, CancellationToken ct = default)
    {
        var cotacao = await GetByAtivoIdAndDataAsync(ativoId, dataRef, ct);
        if (cotacao != null)
        {
            _db.CotacoesAtivo.Remove(cotacao);
            await _db.SaveChangesAsync(ct);
        }
    }
}
