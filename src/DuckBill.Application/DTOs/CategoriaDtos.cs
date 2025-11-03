namespace DuckBill.Application.DTOs;
public record CategoriaCreateDto(string Nome);
public record CategoriaDto(long Id, string Nome, Dictionary<string, string>? _links = null);
