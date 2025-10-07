namespace DuckBill.Application.DTOs;
public record TransacaoCreateDto(long UsuarioId,long AtivoId,string Tipo,decimal Qtd,decimal Preco,DateTime DataNegocio);
public record TransacaoDto(long Id,long UsuarioId,long AtivoId,string Tipo,decimal Qtd,decimal Preco,DateTime DataNegocio);
