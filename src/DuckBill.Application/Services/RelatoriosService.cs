using DuckBill.Application.DTOs;
using DuckBill.Application.Observability;
using DuckBill.Domain.Interfaces;
using DuckBill.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DuckBill.Application.Services;

public class RelatoriosService
{
    private readonly DuckBillDbContext _db;
    private readonly ICambioService _cambio;
    public RelatoriosService(DuckBillDbContext db, ICambioService cambio)
    {
        _db = db; _cambio = cambio;
    }

    public async Task<Top3GastosResponse> Top3GastosDoMesAsync(long usuarioId,int mes,int ano,string moedaAlvo,CancellationToken ct=default)
    {
        using var activity = Telemetry.ActivitySource.StartActivity("RelatoriosService.Top3GastosDoMesAsync");
        var inicio = new DateTime(ano, mes, 1);
        var fim = inicio.AddMonths(1);
        var agregados = await _db.Despesas.AsNoTracking()
            .Where(d => d.UsuarioId == usuarioId && d.DataCompra >= inicio && d.DataCompra < fim)
            .GroupBy(d => new { d.CategoriaId, d.Moeda, CategoriaNome = d.Categoria!.Nome })
            .Select(g => new { g.Key.CategoriaId, g.Key.CategoriaNome, g.Key.Moeda, Total = g.Sum(x => x.Valor) })
            .ToListAsync(ct);

        var itens = new List<TopGastoDto>();
        foreach (var a in agregados)
        {
            var totalConv = a.Moeda.Equals(moedaAlvo, StringComparison.OrdinalIgnoreCase)
                ? a.Total
                : await _cambio.ConverterAsync(a.Total, a.Moeda, moedaAlvo, ct);
            itens.Add(new TopGastoDto(a.CategoriaNome, totalConv, moedaAlvo));
        }

        var top3 = itens.OrderByDescending(x => x.TotalConvertido).Take(3).ToList();
        return new Top3GastosResponse(top3, mes, ano, usuarioId, moedaAlvo.ToUpperInvariant());
    }
}
