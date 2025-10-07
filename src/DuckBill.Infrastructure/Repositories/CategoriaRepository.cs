using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DuckBill.Infrastructure.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly DuckBillDbContext _db;

    public CategoriaRepository(DuckBillDbContext db)
    {
        _db = db;
    }

    public async Task<Categoria?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await _db.Categorias.FindAsync(new object[] { id }, ct);
    }

    public async Task<Categoria?> GetByNomeAsync(string nome, CancellationToken ct = default)
    {
        return await _db.Categorias.FirstOrDefaultAsync(c => c.Nome == nome, ct);
    }

    public async Task<IEnumerable<Categoria>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Categorias.ToListAsync(ct);
    }

    public async Task AddAsync(Categoria categoria, CancellationToken ct = default)
    {
        await _db.Categorias.AddAsync(categoria, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Categoria categoria, CancellationToken ct = default)
    {
        _db.Categorias.Update(categoria);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var categoria = await GetByIdAsync(id, ct);
        if (categoria != null)
        {
            _db.Categorias.Remove(categoria);
            await _db.SaveChangesAsync(ct);
        }
    }
}
