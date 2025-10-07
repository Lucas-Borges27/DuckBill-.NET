namespace DuckBill.Application.DTOs;
public record DespesaCreateDto(long UsuarioId,long CategoriaId,decimal Valor,string Moeda,DateTime DataCompra,string? Descricao);
public record DespesaDto(long Id,long UsuarioId,long CategoriaId,decimal Valor,string Moeda,DateTime DataCompra,string? Descricao,string? CategoriaNome);
