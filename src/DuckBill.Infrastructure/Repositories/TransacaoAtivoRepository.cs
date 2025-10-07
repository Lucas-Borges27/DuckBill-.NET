using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DuckBill.Infrastructure.Repositories;

public class TransacaoAtivoRepository : ITransacaoAtivoRepository
{
    private readonly DuckBillDbContext _db;

    public TransacaoAtivoRepository(DuckBillDbContext db)
    {
        _db = db;
    }

    public async Task<TransacaoAtivo?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await _db.TransacoesAtivo.FindAsync(new object[] { id }, ct);
    }

    public async Task<IEnumerable<TransacaoAtivo>> GetByUsuarioIdAsync(long usuarioId, CancellationToken ct = default)
    {
        return await _db.TransacoesAtivo.Where(t => t.UsuarioId == usuarioId).ToListAsync(ct);
    }

    public async Task AddAsync(TransacaoAtivo transacao, CancellationToken ct = default)
    {
        await _db.TransacoesAtivo.AddAsync(transacao, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TransacaoAtivo transacao, CancellationToken ct = default)
    {
        _db.TransacoesAtivo.Update(transacao);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var transacao = await GetByIdAsync(id, ct);
        if (transacao != null)
        {
            _db.TransacoesAtivo.Remove(transacao);
            await _db.SaveChangesAsync(ct);
        }
    }
}
