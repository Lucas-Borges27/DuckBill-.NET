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

    public async Task<AtivoDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var ativo = await _ativoRepository.GetByIdAsync(id, ct);
        return ativo == null ? null : new AtivoDto(ativo.Id, ativo.Ticker, ativo.Tipo, ativo.MoedaBase);
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
