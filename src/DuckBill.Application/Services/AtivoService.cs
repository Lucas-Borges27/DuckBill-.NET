using DuckBill.Application.DTOs;
using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;

namespace DuckBill.Application.Services;

public class AtivoService
{
    private readonly IAtivoRepository _ativoRepository;

    public AtivoService(IAtivoRepository ativoRepository)
    {
        _ativoRepository = ativoRepository;
    }

    public async Task<PaginatedResponse<AtivoDto>> SearchAsync(string? filter, string? sort, int page = 1, int size = 10, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (size < 1 || size > 100) size = 10;

        var query = await _ativoRepository.GetAllAsync(ct);
        var filtered = query.AsQueryable();

        // Filter
        if (!string.IsNullOrWhiteSpace(filter))
        {
            filtered = filtered.Where(a => a.Ticker.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                           a.Tipo.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                           a.MoedaBase.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        // Sort
        if (!string.IsNullOrWhiteSpace(sort))
        {
            var sortParts = sort.Split(',');
            var sortBy = sortParts[0].Trim();
            var sortDir = sortParts.Length > 1 ? sortParts[1].Trim().ToLower() : "asc";

            filtered = sortBy.ToLower() switch
            {
                "ticker" => sortDir == "desc" ? filtered.OrderByDescending(a => a.Ticker) : filtered.OrderBy(a => a.Ticker),
                "tipo" => sortDir == "desc" ? filtered.OrderByDescending(a => a.Tipo) : filtered.OrderBy(a => a.Tipo),
                "moedabase" => sortDir == "desc" ? filtered.OrderByDescending(a => a.MoedaBase) : filtered.OrderBy(a => a.MoedaBase),
                _ => filtered.OrderBy(a => a.Id)
            };
        }
        else
        {
            filtered = filtered.OrderBy(a => a.Id);
        }

        var totalItems = filtered.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / size);
        var items = filtered.Skip((page - 1) * size).Take(size).ToList();

        var dtos = items.Select(a => new AtivoDto(a.Id, a.Ticker, a.Tipo, a.MoedaBase)).ToList();

        var links = new Dictionary<string, string>
        {
            ["self"] = $"/api/ativos/search?filter={filter}&sort={sort}&page={page}&size={size}"
        };
        if (page > 1) links["prev"] = $"/api/ativos/search?filter={filter}&sort={sort}&page={page - 1}&size={size}";
        if (page < totalPages) links["next"] = $"/api/ativos/search?filter={filter}&sort={sort}&page={page + 1}&size={size}";
        links["first"] = $"/api/ativos/search?filter={filter}&sort={sort}&page=1&size={size}";
        links["last"] = $"/api/ativos/search?filter={filter}&sort={sort}&page={totalPages}&size={size}";

        return new PaginatedResponse<AtivoDto>(dtos, page, size, totalPages, totalItems, links);
    }

    public async Task<AtivoDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var ativo = await _ativoRepository.GetByIdAsync(id, ct);
        return ativo == null ? null : new AtivoDto(ativo.Id, ativo.Ticker, ativo.Tipo, ativo.MoedaBase, new Dictionary<string, string> { ["self"] = $"/api/ativos/{id}" });
    }

    public async Task<AtivoDto?> GetByTickerAsync(string ticker, CancellationToken ct = default)
    {
        var ativo = await _ativoRepository.GetByTickerAsync(ticker, ct);
        return ativo == null ? null : new AtivoDto(ativo.Id, ativo.Ticker, ativo.Tipo, ativo.MoedaBase);
    }

    public async Task<IEnumerable<AtivoDto>> GetAllAsync(CancellationToken ct = default)
    {
        var ativos = await _ativoRepository.GetAllAsync(ct);
        return ativos.Select(a => new AtivoDto(a.Id, a.Ticker, a.Tipo, a.MoedaBase));
    }

    public async Task<AtivoDto> CreateAsync(AtivoCreateDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Ticker)) throw new ArgumentException("Ticker é obrigatório.");
        if (string.IsNullOrWhiteSpace(dto.Tipo)) throw new ArgumentException("Tipo é obrigatório.");
        if (string.IsNullOrWhiteSpace(dto.MoedaBase)) throw new ArgumentException("Moeda base é obrigatória.");

        var existing = await _ativoRepository.GetByTickerAsync(dto.Ticker, ct);
        if (existing != null) throw new InvalidOperationException("Ativo já cadastrado.");

        var ativo = new Ativo { Ticker = dto.Ticker, Tipo = dto.Tipo, MoedaBase = dto.MoedaBase };
        await _ativoRepository.AddAsync(ativo, ct);
        return new AtivoDto(ativo.Id, ativo.Ticker, ativo.Tipo, ativo.MoedaBase);
    }

    public async Task UpdateAsync(long id, AtivoCreateDto dto, CancellationToken ct = default)
    {
        var ativo = await _ativoRepository.GetByIdAsync(id, ct);
        if (ativo == null) throw new KeyNotFoundException("Ativo não encontrado.");

        if (string.IsNullOrWhiteSpace(dto.Ticker)) throw new ArgumentException("Ticker é obrigatório.");
        if (string.IsNullOrWhiteSpace(dto.Tipo)) throw new ArgumentException("Tipo é obrigatório.");
        if (string.IsNullOrWhiteSpace(dto.MoedaBase)) throw new ArgumentException("Moeda base é obrigatória.");

        var existing = await _ativoRepository.GetByTickerAsync(dto.Ticker, ct);
        if (existing != null && existing.Id != id) throw new InvalidOperationException("Ativo já cadastrado.");

        ativo.Ticker = dto.Ticker;
        ativo.Tipo = dto.Tipo;
        ativo.MoedaBase = dto.MoedaBase;
        await _ativoRepository.UpdateAsync(ativo, ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var ativo = await _ativoRepository.GetByIdAsync(id, ct);
        if (ativo == null) throw new KeyNotFoundException("Ativo não encontrado.");

        await _ativoRepository.DeleteAsync(id, ct);
    }
}
