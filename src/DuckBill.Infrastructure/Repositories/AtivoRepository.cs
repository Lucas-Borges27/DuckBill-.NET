using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DuckBill.Infrastructure.Repositories;

public class AtivoRepository : IAtivoRepository
{
    private readonly DuckBillDbContext _db;

    public AtivoRepository(DuckBillDbContext db)
    {
        _db = db;
    }

    public async Task<Ativo?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await _db.Ativos.FindAsync(new object[] { id }, ct);
    }

    public async Task<Ativo?> GetByTickerAsync(string ticker, CancellationToken ct = default)
    {
        return await _db.Ativos.FirstOrDefaultAsync(a => a.Ticker == ticker, ct);
    }

    public async Task<IEnumerable<Ativo>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Ativos.ToListAsync(ct);
    }

    public async Task AddAsync(Ativo ativo, CancellationToken ct = default)
    {
        await _db.Ativos.AddAsync(ativo, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Ativo ativo, CancellationToken ct = default)
    {
        _db.Ativos.Update(ativo);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var ativo = await GetByIdAsync(id, ct);
        if (ativo != null)
        {
            _db.Ativos.Remove(ativo);
            await _db.SaveChangesAsync(ct);
        }
    }
}
