using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DuckBill.Infrastructure.Repositories;

public class DespesaMongoRepository : IDespesaMongoRepository
{
    private readonly IMongoCollection<DespesaMongo> _collection;

    public DespesaMongoRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<DespesaMongo>("despesas");
    }

    public async Task<IEnumerable<Despesa>> GetAllAsync(CancellationToken ct = default)
    {
        var despesasMongo = await _collection.Find(_ => true).ToListAsync(ct);
        return despesasMongo.Select(MapToDomain);
    }

    public async Task<Despesa?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        var despesaMongo = await _collection.Find(d => d.Id == objectId).FirstOrDefaultAsync(ct);
        return despesaMongo != null ? MapToDomain(despesaMongo) : null;
    }

    public async Task AddAsync(Despesa despesa, CancellationToken ct = default)
    {
        var despesaMongo = new DespesaMongo
        {
            UsuarioId = despesa.UsuarioId,
            CategoriaId = despesa.CategoriaId,
            Valor = despesa.Valor,
            Moeda = despesa.Moeda,
            DataCompra = despesa.DataCompra,
            Descricao = despesa.Descricao
        };

        await _collection.InsertOneAsync(despesaMongo, cancellationToken: ct);
    }

    private static Despesa MapToDomain(DespesaMongo despesaMongo)
    {
        return new Despesa
        {
            Id = 0, // MongoDB uses ObjectId, not long
            UsuarioId = despesaMongo.UsuarioId,
            CategoriaId = despesaMongo.CategoriaId,
            Valor = despesaMongo.Valor,
            Moeda = despesaMongo.Moeda,
            DataCompra = despesaMongo.DataCompra,
            Descricao = despesaMongo.Descricao
        };
    }

    private class DespesaMongo
    {
        [BsonId]
        public ObjectId Id { get; set; }
        
        [BsonElement("usuarioId")]
        public long UsuarioId { get; set; }
        
        [BsonElement("categoriaId")]
        public long CategoriaId { get; set; }
        
        [BsonElement("valor")]
        public decimal Valor { get; set; }
        
        [BsonElement("moeda")]
        public string Moeda { get; set; } = "BRL";
        
        [BsonElement("dataCompra")]
        public DateTime DataCompra { get; set; }
        
        [BsonElement("descricao")]
        public string? Descricao { get; set; }
    }
}

// Made with Bob
