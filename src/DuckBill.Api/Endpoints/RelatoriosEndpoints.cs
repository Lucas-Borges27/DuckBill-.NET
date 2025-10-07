using DuckBill.Application.Services;
using DuckBill.Domain.Interfaces;

namespace DuckBill.Api.Endpoints;

public static class RelatoriosEndpoints
{
    public static IEndpointRouteBuilder MapRelatorios(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/api/relatorios");
        grp.MapGet("top3-gastos", async (long usuarioId, int mes, int ano, string moeda, RelatoriosService svc, CancellationToken ct) =>
        {
            var resp = await svc.Top3GastosDoMesAsync(usuarioId, mes, ano, moeda.ToUpperInvariant(), ct);
            return Results.Ok(resp);
        });

        grp.MapGet("cambio", async (string from, string to, decimal valor, ICambioService svc, CancellationToken ct) =>
        {
            var convertido = await svc.ConverterAsync(valor, from, to, ct);
            return Results.Ok(new { from, to, valor, convertido });
        });

        return app;
    }
}
