using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DuckBill.Infrastructure.Repositories;

public class DespesaRepository : IDespesaRepository
{
    private readonly DuckBillDbContext _db;

    public DespesaRepository(DuckBillDbContext db)
    {
        _db = db;
    }

    public async Task<Despesa?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await _db.Despesas.FindAsync(new object[] { id }, ct);
    }

    public async Task<IEnumerable<Despesa>> GetByUsuarioIdAsync(long usuarioId, CancellationToken ct = default)
    {
        return await _db.Despesas.Where(d => d.UsuarioId == usuarioId).ToListAsync(ct);
    }

    public async Task<IEnumerable<Despesa>> GetByUsuarioIdAndMesAnoAsync(long usuarioId, int mes, int ano, CancellationToken ct = default)
    {
        return await _db.Despesas.Where(d => d.UsuarioId == usuarioId && d.DataCompra.Month == mes && d.DataCompra.Year == ano).ToListAsync(ct);
    }

    public async Task AddAsync(Despesa despesa, CancellationToken ct = default)
    {
        await _db.Despesas.AddAsync(despesa, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Despesa despesa, CancellationToken ct = default)
    {
        _db.Despesas.Update(despesa);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var despesa = await GetByIdAsync(id, ct);
        if (despesa != null)
        {
            _db.Despesas.Remove(despesa);
            await _db.SaveChangesAsync(ct);
        }
    }
}
