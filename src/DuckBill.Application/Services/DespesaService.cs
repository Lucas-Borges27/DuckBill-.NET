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

    public async Task<DespesaDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var despesa = await _despesaRepository.GetByIdAsync(id, ct);
        if (despesa == null) return null;
        var categoria = await _categoriaRepository.GetByIdAsync(despesa.CategoriaId, ct);
        return new DespesaDto(despesa.Id, despesa.UsuarioId, despesa.CategoriaId, despesa.Valor, despesa.Moeda, despesa.DataCompra, despesa.Descricao, categoria?.Nome);
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
