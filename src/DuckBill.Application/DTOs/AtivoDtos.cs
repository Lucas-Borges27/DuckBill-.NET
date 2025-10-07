namespace DuckBill.Application.DTOs;
public record AtivoCreateDto(string Ticker,string Tipo,string MoedaBase="BRL");
public record AtivoDto(long Id,string Ticker,string Tipo,string MoedaBase);
