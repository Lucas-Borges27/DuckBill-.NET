using System.ComponentModel.DataAnnotations.Schema;

namespace DuckBill.Domain.Entities;

public class Ativo
{
    [Column("ID")]
    public long Id { get; set; }
    [Column("TICKER")]
    public string Ticker { get; set; } = default!;
    [Column("TIPO")]
    public string Tipo { get; set; } = default!; // Ação/ETF/Cripto
    [Column("MOEDA_BASE")]
    public string MoedaBase { get; set; } = "BRL";
    public ICollection<TransacaoAtivo> Transacoes { get; set; } = new List<TransacaoAtivo>();
    public ICollection<CotacaoAtivo> Cotacoes { get; set; } = new List<CotacaoAtivo>();
}
