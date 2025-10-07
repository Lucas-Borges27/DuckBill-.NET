using System.ComponentModel.DataAnnotations.Schema;

namespace DuckBill.Domain.Entities;

public class TransacaoAtivo
{
    [Column("ID")]
    public long Id { get; set; }
    [Column("USUARIO_ID")]
    public long UsuarioId { get; set; }
    [Column("ATIVO_ID")]
    public long AtivoId { get; set; }
    [Column("TIPO")]
    public string Tipo { get; set; } = default!; // BUY | SELL
    [Column("QTD")]
    public decimal Qtd { get; set; }
    [Column("PRECO")]
    public decimal Preco { get; set; }
    [Column("DATA_NEGOCIO")]
    public DateTime DataNegocio { get; set; }
    public Usuario? Usuario { get; set; }
    public Ativo? Ativo { get; set; }
}
