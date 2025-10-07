namespace DuckBill.Application.DTOs;
public record TopGastoDto(string Categoria, decimal TotalConvertido, string MoedaAlvo);
public record Top3GastosResponse(IReadOnlyList<TopGastoDto> Itens, int Mes, int Ano, long UsuarioId, string MoedaAlvo);
