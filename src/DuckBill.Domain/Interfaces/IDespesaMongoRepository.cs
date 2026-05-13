using DuckBill.Domain.Entities;

namespace DuckBill.Domain.Interfaces;

public interface IDespesaMongoRepository
{
    Task<IEnumerable<Despesa>> GetAllAsync(CancellationToken ct = default);
    Task<Despesa?> GetByIdAsync(string id, CancellationToken ct = default);
    Task AddAsync(Despesa despesa, CancellationToken ct = default);
}

// Made with Bob
