using DuckBill.Application.DTOs;
using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;

namespace DuckBill.Application.Services;

public class CategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriaService(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<PaginatedResponse<CategoriaDto>> SearchAsync(string? filter, string? sort, int page = 1, int size = 10, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (size < 1 || size > 100) size = 10;

        var query = await _categoriaRepository.GetAllAsync(ct);
        var filtered = query.AsQueryable();

        // Filter
        if (!string.IsNullOrWhiteSpace(filter))
        {
            filtered = filtered.Where(c => c.Nome.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        // Sort
        if (!string.IsNullOrWhiteSpace(sort))
        {
            var sortParts = sort.Split(',');
            var sortBy = sortParts[0].Trim();
            var sortDir = sortParts.Length > 1 ? sortParts[1].Trim().ToLower() : "asc";

            filtered = sortBy.ToLower() switch
            {
                "nome" => sortDir == "desc" ? filtered.OrderByDescending(c => c.Nome) : filtered.OrderBy(c => c.Nome),
                _ => filtered.OrderBy(c => c.Id)
            };
        }
        else
        {
            filtered = filtered.OrderBy(c => c.Id);
        }

        var totalItems = filtered.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / size);
        var items = filtered.Skip((page - 1) * size).Take(size).ToList();

        var dtos = items.Select(c => new CategoriaDto(c.Id, c.Nome)).ToList();

        var links = new Dictionary<string, string>
        {
            ["self"] = $"/api/categorias/search?filter={filter}&sort={sort}&page={page}&size={size}"
        };
        if (page > 1) links["prev"] = $"/api/categorias/search?filter={filter}&sort={sort}&page={page - 1}&size={size}";
        if (page < totalPages) links["next"] = $"/api/categorias/search?filter={filter}&sort={sort}&page={page + 1}&size={size}";
        links["first"] = $"/api/categorias/search?filter={filter}&sort={sort}&page=1&size={size}";
        links["last"] = $"/api/categorias/search?filter={filter}&sort={sort}&page={totalPages}&size={size}";

        return new PaginatedResponse<CategoriaDto>(dtos, page, size, totalPages, totalItems, links);
    }

    public async Task<CategoriaDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(id, ct);
        return categoria == null ? null : new CategoriaDto(categoria.Id, categoria.Nome, new Dictionary<string, string> { ["self"] = $"/api/categorias/{id}" });
    }

    public async Task<CategoriaDto?> GetByNomeAsync(string nome, CancellationToken ct = default)
    {
        var categoria = await _categoriaRepository.GetByNomeAsync(nome, ct);
        return categoria == null ? null : new CategoriaDto(categoria.Id, categoria.Nome);
    }

    public async Task<IEnumerable<CategoriaDto>> GetAllAsync(CancellationToken ct = default)
    {
        var categorias = await _categoriaRepository.GetAllAsync(ct);
        return categorias.Select(c => new CategoriaDto(c.Id, c.Nome));
    }

    public async Task<CategoriaDto> CreateAsync(CategoriaCreateDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome)) throw new ArgumentException("Nome é obrigatório.");

        var existing = await _categoriaRepository.GetByNomeAsync(dto.Nome, ct);
        if (existing != null) throw new InvalidOperationException("Categoria já cadastrada.");

        var categoria = new Categoria { Nome = dto.Nome };
        await _categoriaRepository.AddAsync(categoria, ct);
        return new CategoriaDto(categoria.Id, categoria.Nome);
    }

    public async Task UpdateAsync(long id, CategoriaCreateDto dto, CancellationToken ct = default)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(id, ct);
        if (categoria == null) throw new KeyNotFoundException("Categoria não encontrada.");

        if (string.IsNullOrWhiteSpace(dto.Nome)) throw new ArgumentException("Nome é obrigatório.");

        var existing = await _categoriaRepository.GetByNomeAsync(dto.Nome, ct);
        if (existing != null && existing.Id != id) throw new InvalidOperationException("Categoria já cadastrada.");

        categoria.Nome = dto.Nome;
        await _categoriaRepository.UpdateAsync(categoria, ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(id, ct);
        if (categoria == null) throw new KeyNotFoundException("Categoria não encontrada.");

        await _categoriaRepository.DeleteAsync(id, ct);
    }
}
