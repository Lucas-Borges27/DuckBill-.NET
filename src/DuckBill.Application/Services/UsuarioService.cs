using DuckBill.Application.DTOs;
using DuckBill.Domain.Entities;
using DuckBill.Domain.Interfaces;

namespace DuckBill.Application.Services;

public class UsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<PaginatedResponse<UsuarioDto>> SearchAsync(string? filter, string? sort, int page = 1, int size = 10, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (size < 1 || size > 100) size = 10;

        var query = await _usuarioRepository.GetAllAsync(ct);
        var filtered = query.AsQueryable();

        // Filter
        if (!string.IsNullOrWhiteSpace(filter))
        {
            filtered = filtered.Where(u => u.Nome.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                           u.Email.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        // Sort
        if (!string.IsNullOrWhiteSpace(sort))
        {
            var sortParts = sort.Split(',');
            var sortBy = sortParts[0].Trim();
            var sortDir = sortParts.Length > 1 ? sortParts[1].Trim().ToLower() : "asc";

            filtered = sortBy.ToLower() switch
            {
                "nome" => sortDir == "desc" ? filtered.OrderByDescending(u => u.Nome) : filtered.OrderBy(u => u.Nome),
                "email" => sortDir == "desc" ? filtered.OrderByDescending(u => u.Email) : filtered.OrderBy(u => u.Email),
                _ => filtered.OrderBy(u => u.Id)
            };
        }
        else
        {
            filtered = filtered.OrderBy(u => u.Id);
        }

        var totalItems = filtered.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / size);
        var items = filtered.Skip((page - 1) * size).Take(size).ToList();

        var dtos = items.Select(u => new UsuarioDto(u.Id, u.Nome, u.Email)).ToList();

        var links = new Dictionary<string, string>
        {
            ["self"] = $"/api/usuarios/search?filter={filter}&sort={sort}&page={page}&size={size}"
        };
        if (page > 1) links["prev"] = $"/api/usuarios/search?filter={filter}&sort={sort}&page={page - 1}&size={size}";
        if (page < totalPages) links["next"] = $"/api/usuarios/search?filter={filter}&sort={sort}&page={page + 1}&size={size}";
        links["first"] = $"/api/usuarios/search?filter={filter}&sort={sort}&page=1&size={size}";
        links["last"] = $"/api/usuarios/search?filter={filter}&sort={sort}&page={totalPages}&size={size}";

        return new PaginatedResponse<UsuarioDto>(dtos, page, size, totalPages, totalItems, links);
    }

    public async Task<UsuarioDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        return usuario == null ? null : new UsuarioDto(usuario.Id, usuario.Nome, usuario.Email, new Dictionary<string, string> { ["self"] = $"/api/usuarios/{id}" });
    }

    public async Task<UsuarioDto?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(email, ct);
        return usuario == null ? null : new UsuarioDto(usuario.Id, usuario.Nome, usuario.Email);
    }

    public async Task<IEnumerable<UsuarioDto>> GetAllAsync(CancellationToken ct = default)
    {
        var usuarios = await _usuarioRepository.GetAllAsync(ct);
        return usuarios.Select(u => new UsuarioDto(u.Id, u.Nome, u.Email));
    }

    public async Task<UsuarioDto> CreateAsync(UsuarioCreateDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome)) throw new ArgumentException("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Email é obrigatório.");
        if (string.IsNullOrWhiteSpace(dto.Senha)) throw new ArgumentException("Senha é obrigatória.");

        var existing = await _usuarioRepository.GetByEmailAsync(dto.Email, ct);
        if (existing != null) throw new InvalidOperationException("Email já cadastrado.");

        var usuario = new Usuario { Nome = dto.Nome, Email = dto.Email, Senha = dto.Senha };
        await _usuarioRepository.AddAsync(usuario, ct);
        return new UsuarioDto(usuario.Id, usuario.Nome, usuario.Email);
    }

    public async Task UpdateAsync(long id, UsuarioCreateDto dto, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        if (usuario == null) throw new KeyNotFoundException("Usuário não encontrado.");

        if (string.IsNullOrWhiteSpace(dto.Nome)) throw new ArgumentException("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(dto.Email)) throw new ArgumentException("Email é obrigatório.");
        if (string.IsNullOrWhiteSpace(dto.Senha)) throw new ArgumentException("Senha é obrigatória.");

        var existing = await _usuarioRepository.GetByEmailAsync(dto.Email, ct);
        if (existing != null && existing.Id != id) throw new InvalidOperationException("Email já cadastrado.");

        usuario.Nome = dto.Nome;
        usuario.Email = dto.Email;
        usuario.Senha = dto.Senha;
        await _usuarioRepository.UpdateAsync(usuario, ct);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        if (usuario == null) throw new KeyNotFoundException("Usuário não encontrado.");

        await _usuarioRepository.DeleteAsync(id, ct);
    }
}
