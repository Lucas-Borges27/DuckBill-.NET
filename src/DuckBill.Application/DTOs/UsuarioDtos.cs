namespace DuckBill.Application.DTOs;
public record UsuarioCreateDto(string Nome, string Email, string Senha);
public record UsuarioDto(long Id, string Nome, string Email);
