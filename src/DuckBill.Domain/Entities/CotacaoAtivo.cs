using System.ComponentModel.DataAnnotations.Schema;

namespace DuckBill.Domain.Entities;

public class CotacaoAtivo
{
    [Column("ATIVO_ID")]
    public long AtivoId { get; set; }
    [Column("DATA_REF")]
    public DateTime DataRef { get; set; }
    [Column("PRECO_FECH")]
    public decimal PrecoFech { get; set; }
    public Ativo? Ativo { get; set; }
}
