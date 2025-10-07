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

    public async Task<CategoriaDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(id, ct);
        return categoria == null ? null : new CategoriaDto(categoria.Id, categoria.Nome);
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
