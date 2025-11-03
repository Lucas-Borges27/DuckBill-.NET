using DuckBill.Application.DTOs;
using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;

namespace DuckBill.Application.Services;

public class DespesaService
{
    private readonly IDespesaRepository _despesaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public DespesaService(IDespesaRepository despesaRepository, IUsuarioRepository usuarioRepository, ICategoriaRepository categoriaRepository)
    {
        _despesaRepository = despesaRepository;
        _usuarioRepository = usuarioRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<PaginatedResponse<DespesaDto>> SearchAsync(long? usuarioId, string? filter, string? sort, int page = 1, int size = 10, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (size < 1 || size > 100) size = 10;

        var query = usuarioId.HasValue ? await _despesaRepository.GetByUsuarioIdAsync(usuarioId.Value, ct) : new List<Despesa>();
        var filtered = query.AsQueryable();

        // Filter
        if (!string.IsNullOrWhiteSpace(filter))
        {
            filtered = filtered.Where(d => d.Descricao != null && d.Descricao.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                           d.Moeda.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        // Sort
        if (!string.IsNullOrWhiteSpace(sort))
        {
            var sortParts = sort.Split(',');
            var sortBy = sortParts[0].Trim();
            var sortDir = sortParts.Length > 1 ? sortParts[1].Trim().ToLower() : "asc";

            filtered = sortBy.ToLower() switch
            {
                "valor" => sortDir == "desc" ? filtered.OrderByDescending(d => d.Valor) : filtered.OrderBy(d => d.Valor),
                "datacompra" => sortDir == "desc" ? filtered.OrderByDescending(d => d.DataCompra) : filtered.OrderBy(d => d.DataCompra),
                "moeda" => sortDir == "desc" ? filtered.OrderByDescending(d => d.Moeda) : filtered.OrderBy(d => d.Moeda),
                _ => filtered.OrderBy(d => d.Id)
            };
        }
        else
        {
            filtered = filtered.OrderBy(d => d.Id);
        }

        var totalItems = filtered.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / size);
        var items = filtered.Skip((page - 1) * size).Take(size).ToList();

        var dtos = new List<DespesaDto>();
        foreach (var despesa in items)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(despesa.CategoriaId, ct);
            dtos.Add(new DespesaDto(despesa.Id, despesa.UsuarioId, despesa.CategoriaId, despesa.Valor, despesa.Moeda, despesa.DataCompra, despesa.Descricao, categoria?.Nome));
        }

        var links = new Dictionary<string, string>
        {
            ["self"] = $"/api/despesas/search?usuarioId={usuarioId}&filter={filter}&sort={sort}&page={page}&size={size}"
        };
        if (page > 1) links["prev"] = $"/api/despesas/search?usuarioId={usuarioId}&filter={filter}&sort={sort}&page={page - 1}&size={size}";
        if (page < totalPages) links["next"] = $"/api/despesas/search?usuarioId={usuarioId}&filter={filter}&sort={sort}&page={page + 1}&size={size}";
        links["first"] = $"/api/despesas/search?usuarioId={usuarioId}&filter={filter}&sort={sort}&page=1&size={size}";
        links["last"] = $"/api/despesas/search?usuarioId={usuarioId}&filter={filter}&sort={sort}&page={totalPages}&size={size}";

        return new PaginatedResponse<DespesaDto>(dtos, page, size, totalPages, totalItems, links);
    }

    public async Task<DespesaDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var despesa = await _despesaRepository.GetByIdAsync(id, ct);
        if (despesa == null) return null;
        var categoria = await _categoriaRepository.GetByIdAsync(despesa.CategoriaId, ct);
        return new DespesaDto(despesa.Id, despesa.UsuarioId, despesa.CategoriaId, despesa.Valor, despesa.Moeda, despesa.DataCompra, despesa.Descricao, categoria?.Nome, new Dictionary<string, string> { ["self"] = $"/api/despesas/{id}" });
    }

    public async Task<IEnumerable<DespesaDto>> GetByUsuarioIdAsync(long usuarioId, CancellationToken ct = default)
    {
        var despesas = await _despesaRepository.GetByUsuarioIdAsync(usuarioId, ct);
        var result = new List<DespesaDto>();
        foreach (var despesa in despesas)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(despesa.CategoriaId, ct);
            result.Add(new DespesaDto(despesa.Id, despesa.UsuarioId, despesa.CategoriaId, despesa.Valor, despesa.Moeda, despesa.DataCompra, despesa.Descricao, categoria?.Nome));
        }
        return result;
    }

    public async Task<IEnumerable<DespesaDto>> GetByUsuarioIdAndMesAnoAsync(long usuarioId, int mes, int ano, CancellationToken ct = default)
    {
        var despesas = await _despesaRepository.GetByUsuarioIdAndMesAnoAsync(usuarioId, mes, ano, ct);
        var result = new List<DespesaDto>();
        foreach (var despesa in despesas)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(despesa.CategoriaId, ct);
            result.Add(new DespesaDto(despesa.Id, despesa.UsuarioId, despesa.CategoriaId, despesa.Valor, despesa.Moeda, despesa.DataCompra, despesa.Descricao, categoria?.Nome));
        }
        return result;
    }

    public async Task<DespesaDto> CreateAsync(DespesaCreateDto dto, CancellationToken ct = default)
    {
        if (dto.Valor <= 0) throw new ArgumentException("Valor deve ser maior que zero.");
        if (string.IsNullOrWhiteSpace(dto.Moeda)) throw new ArgumentException("Moeda é obrigatória.");
        if (dto.DataCompra > DateTime.Now) throw new ArgumentException("Data de compra não pode ser futura.");

        var usuario = await _usuarioRepository.GetByIdAsync(dto.UsuarioId, ct);
        if (usuario == null) throw new KeyNotFoundException("Usuário não encontrado.");

        var categoria = await _categoriaRepository.GetByIdAsync(dto.CategoriaId, ct);
        if (categoria == null) throw new KeyNotFoundException("Categoria não encontrada.");

        var despesa = new Despesa
        {
            UsuarioId = dto.UsuarioId,
            CategoriaId = dto.CategoriaId,
            Valor = dto.Valor,
            Moeda = dto.Moeda,
            DataCompra = dto.DataCompra,
            Descricao = dto.Descricao
        };
        await _despesaRepository.AddAsync(despesa, ct);
        return new DespesaDto(despesa.Id, despesa.UsuarioId, despesa.CategoriaId, despesa.Valor, despesa.Moeda, despesa.DataCompra, despesa.Descricao, categoria.Nome);
    }

    public async Task UpdateAsync(long id, DespesaCreateDto dto, CancellationToken ct = default)
    {
        var despesa = await _despesaRepository.GetByIdAsync(id, ct);
        if (despesa == null) throw new KeyNotFoundException("Despesa não encontrada.");

        if (dto.Valor <= 0) throw new ArgumentException("Valor deve ser maior que zero.");
        if (string.IsNullOrWhiteSpace(dto.Moeda)) throw new ArgumentException("Moeda é obrigatória.");
        if (dto.DataCompra > DateTime.Now) throw new ArgumentException("Data de compra não pode ser futura.");

        var usuario = await _usuarioRepository.GetByIdAsync(dto.UsuarioId, ct);
        if (usuario == null) throw new KeyNotFoundException("Usuário não encontrado.");

        var categoria = await _categoriaRepository.GetByIdAsync(dto.CategoriaId, ct);
        if (categoria == null) throw new KeyNotFoundException("Categoria não encontrada.");

        despesa.UsuarioId = dto.UsuarioId;
        despesa.CategoriaId = dto.CategoriaId;
        despesa.Valor = dto.Valor;
        despesa.Moeda = dto.Moeda;
        despesa.DataCompra = dto.DataCompra;
        despesa.Descricao = dto.Descricao;
        await _despesaRepository.UpdateAsync(despesa, ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var despesa = await _despesaRepository.GetByIdAsync(id, ct);
        if (despesa == null) throw new KeyNotFoundException("Despesa não encontrada.");

        await _despesaRepository.DeleteAsync(id, ct);
    }
}
