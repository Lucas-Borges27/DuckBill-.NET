using DuckBill.Application.DTOs;
using DuckBill.Application.Observability;
using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;

namespace DuckBill.Application.Services;

public class TransacaoAtivoService
{
    private readonly ITransacaoAtivoRepository _transacaoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAtivoRepository _ativoRepository;

    public TransacaoAtivoService(ITransacaoAtivoRepository transacaoRepository, IUsuarioRepository usuarioRepository, IAtivoRepository ativoRepository)
    {
        _transacaoRepository = transacaoRepository;
        _usuarioRepository = usuarioRepository;
        _ativoRepository = ativoRepository;
    }

    public async Task<PaginatedResponse<TransacaoDto>> SearchAsync(long? usuarioId, string? filter, string? sort, int page = 1, int size = 10, CancellationToken ct = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity("TransacaoAtivoService.SearchAsync");
        if (page < 1) page = 1;
        if (size < 1 || size > 100) size = 10;

        var query = usuarioId.HasValue ? await _transacaoRepository.GetByUsuarioIdAsync(usuarioId.Value, ct) : new List<TransacaoAtivo>();
        var filtered = query.AsQueryable();

        // Filter
        if (!string.IsNullOrWhiteSpace(filter))
        {
            filtered = filtered.Where(t => t.Tipo.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        // Sort
        if (!string.IsNullOrWhiteSpace(sort))
        {
            var sortParts = sort.Split(',');
            var sortBy = sortParts[0].Trim();
            var sortDir = sortParts.Length > 1 ? sortParts[1].Trim().ToLower() : "asc";

            filtered = sortBy.ToLower() switch
            {
                "qtd" => sortDir == "desc" ? filtered.OrderByDescending(t => t.Qtd) : filtered.OrderBy(t => t.Qtd),
                "preco" => sortDir == "desc" ? filtered.OrderByDescending(t => t.Preco) : filtered.OrderBy(t => t.Preco),
                "datanegocio" => sortDir == "desc" ? filtered.OrderByDescending(t => t.DataNegocio) : filtered.OrderBy(t => t.DataNegocio),
                "tipo" => sortDir == "desc" ? filtered.OrderByDescending(t => t.Tipo) : filtered.OrderBy(t => t.Tipo),
                _ => filtered.OrderBy(t => t.Id)
            };
        }
        else
        {
            filtered = filtered.OrderBy(t => t.Id);
        }

        var totalItems = filtered.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / size);
        var items = filtered.Skip((page - 1) * size).Take(size).ToList();

        var dtos = items.Select(t => new TransacaoDto(t.Id, t.UsuarioId, t.AtivoId, t.Tipo, t.Qtd, t.Preco, t.DataNegocio)).ToList();

        var links = new Dictionary<string, string>
        {
            ["self"] = $"/api/transacoes-ativo/search?usuarioId={usuarioId}&filter={filter}&sort={sort}&page={page}&size={size}"
        };
        if (page > 1) links["prev"] = $"/api/transacoes-ativo/search?usuarioId={usuarioId}&filter={filter}&sort={sort}&page={page - 1}&size={size}";
        if (page < totalPages) links["next"] = $"/api/transacoes-ativo/search?usuarioId={usuarioId}&filter={filter}&sort={sort}&page={page + 1}&size={size}";
        links["first"] = $"/api/transacoes-ativo/search?usuarioId={usuarioId}&filter={filter}&sort={sort}&page=1&size={size}";
        links["last"] = $"/api/transacoes-ativo/search?usuarioId={usuarioId}&filter={filter}&sort={sort}&page={totalPages}&size={size}";

        return new PaginatedResponse<TransacaoDto>(dtos, page, size, totalPages, totalItems, links);
    }

    public async Task<TransacaoDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity("TransacaoAtivoService.GetByIdAsync");
        var transacao = await _transacaoRepository.GetByIdAsync(id, ct);
        return transacao == null ? null : new TransacaoDto(transacao.Id, transacao.UsuarioId, transacao.AtivoId, transacao.Tipo, transacao.Qtd, transacao.Preco, transacao.DataNegocio, new Dictionary<string, string> { ["self"] = $"/api/transacoes-ativo/{id}" });
    }

    public async Task<IEnumerable<TransacaoDto>> GetByUsuarioIdAsync(long usuarioId, CancellationToken ct = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity("TransacaoAtivoService.GetByUsuarioIdAsync");
        var transacoes = await _transacaoRepository.GetByUsuarioIdAsync(usuarioId, ct);
        return transacoes.Select(t => new TransacaoDto(t.Id, t.UsuarioId, t.AtivoId, t.Tipo, t.Qtd, t.Preco, t.DataNegocio));
    }

    public async Task<TransacaoDto> CreateAsync(TransacaoCreateDto dto, CancellationToken ct = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity("TransacaoAtivoService.CreateAsync");
        if (dto.Qtd <= 0) throw new ArgumentException("Quantidade deve ser maior que zero.");
        if (dto.Preco <= 0) throw new ArgumentException("Preço deve ser maior que zero.");
        if (string.IsNullOrWhiteSpace(dto.Tipo) || (dto.Tipo != "BUY" && dto.Tipo != "SELL")) throw new ArgumentException("Tipo deve ser BUY ou SELL.");
        if (dto.DataNegocio > DateTime.Now) throw new ArgumentException("Data de negócio não pode ser futura.");

        var usuario = await _usuarioRepository.GetByIdAsync(dto.UsuarioId, ct);
        if (usuario == null) throw new KeyNotFoundException("Usuário não encontrado.");

        var ativo = await _ativoRepository.GetByIdAsync(dto.AtivoId, ct);
        if (ativo == null) throw new KeyNotFoundException("Ativo não encontrado.");

        var transacao = new TransacaoAtivo
        {
            UsuarioId = dto.UsuarioId,
            AtivoId = dto.AtivoId,
            Tipo = dto.Tipo,
            Qtd = dto.Qtd,
            Preco = dto.Preco,
            DataNegocio = dto.DataNegocio
        };
        await _transacaoRepository.AddAsync(transacao, ct);
        return new TransacaoDto(transacao.Id, transacao.UsuarioId, transacao.AtivoId, transacao.Tipo, transacao.Qtd, transacao.Preco, transacao.DataNegocio);
    }

    public async Task UpdateAsync(long id, TransacaoCreateDto dto, CancellationToken ct = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity("TransacaoAtivoService.UpdateAsync");
        var transacao = await _transacaoRepository.GetByIdAsync(id, ct);
        if (transacao == null) throw new KeyNotFoundException("Transação não encontrada.");

        if (dto.Qtd <= 0) throw new ArgumentException("Quantidade deve ser maior que zero.");
        if (dto.Preco <= 0) throw new ArgumentException("Preço deve ser maior que zero.");
        if (string.IsNullOrWhiteSpace(dto.Tipo) || (dto.Tipo != "BUY" && dto.Tipo != "SELL")) throw new ArgumentException("Tipo deve ser BUY ou SELL.");
        if (dto.DataNegocio > DateTime.Now) throw new ArgumentException("Data de negócio não pode ser futura.");

        var usuario = await _usuarioRepository.GetByIdAsync(dto.UsuarioId, ct);
        if (usuario == null) throw new KeyNotFoundException("Usuário não encontrado.");

        var ativo = await _ativoRepository.GetByIdAsync(dto.AtivoId, ct);
        if (ativo == null) throw new KeyNotFoundException("Ativo não encontrado.");

        transacao.UsuarioId = dto.UsuarioId;
        transacao.AtivoId = dto.AtivoId;
        transacao.Tipo = dto.Tipo;
        transacao.Qtd = dto.Qtd;
        transacao.Preco = dto.Preco;
        transacao.DataNegocio = dto.DataNegocio;
        await _transacaoRepository.UpdateAsync(transacao, ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity("TransacaoAtivoService.DeleteAsync");
        var transacao = await _transacaoRepository.GetByIdAsync(id, ct);
        if (transacao == null) throw new KeyNotFoundException("Transação não encontrada.");

        await _transacaoRepository.DeleteAsync(id, ct);
    }
}
