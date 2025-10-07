using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DuckBill.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly DuckBillDbContext _db;

    public UsuarioRepository(DuckBillDbContext db)
    {
        _db = db;
    }

    public async Task<Usuario?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await _db.Usuarios.FindAsync(new object[] { id }, ct);
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Usuarios.ToListAsync(ct);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken ct = default)
    {
        await _db.Usuarios.AddAsync(usuario, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Usuario usuario, CancellationToken ct = default)
    {
        _db.Usuarios.Update(usuario);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var usuario = await GetByIdAsync(id, ct);
        if (usuario != null)
        {
            _db.Usuarios.Remove(usuario);
            await _db.SaveChangesAsync(ct);
        }
    }
}
