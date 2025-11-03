namespace DuckBill.Application.DTOs;

public record PaginatedResponse<T>(
    IEnumerable<T> Items,
    int Page,
    int Size,
    int TotalPages,
    long TotalItems,
    Dictionary<string, string> _links
);
