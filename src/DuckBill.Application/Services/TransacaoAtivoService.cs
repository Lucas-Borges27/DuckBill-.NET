using DuckBill.Application.DTOs;
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

    public async Task<TransacaoDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var transacao = await _transacaoRepository.GetByIdAsync(id, ct);
        return transacao == null ? null : new TransacaoDto(transacao.Id, transacao.UsuarioId, transacao.AtivoId, transacao.Tipo, transacao.Qtd, transacao.Preco, transacao.DataNegocio);
    }

    public async Task<IEnumerable<TransacaoDto>> GetByUsuarioIdAsync(long usuarioId, CancellationToken ct = default)
    {
        var transacoes = await _transacaoRepository.GetByUsuarioIdAsync(usuarioId, ct);
        return transacoes.Select(t => new TransacaoDto(t.Id, t.UsuarioId, t.AtivoId, t.Tipo, t.Qtd, t.Preco, t.DataNegocio));
    }

    public async Task<TransacaoDto> CreateAsync(TransacaoCreateDto dto, CancellationToken ct = default)
    {
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
        var transacao = await _transacaoRepository.GetByIdAsync(id, ct);
        if (transacao == null) throw new KeyNotFoundException("Transação não encontrada.");

        await _transacaoRepository.DeleteAsync(id, ct);
    }
}
