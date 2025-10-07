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

    public async Task<UsuarioDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, ct);
        return usuario == null ? null : new UsuarioDto(usuario.Id, usuario.Nome, usuario.Email);
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
