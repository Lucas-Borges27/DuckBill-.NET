namespace DuckBill.Domain.Interfaces;

public interface ICambioService
{
    Task<decimal> ConverterAsync(decimal valor, string de, string para, CancellationToken ct = default);
}
