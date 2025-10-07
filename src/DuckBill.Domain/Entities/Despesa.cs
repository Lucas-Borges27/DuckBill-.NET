using System.ComponentModel.DataAnnotations.Schema;

namespace DuckBill.Domain.Entities;

public class Despesa
{
    [Column("ID")]
    public long Id { get; set; }
    [Column("USUARIO_ID")]
    public long UsuarioId { get; set; }
    [Column("CATEGORIA_ID")]
    public long CategoriaId { get; set; }
    [Column("VALOR")]
    public decimal Valor { get; set; }
    [Column("MOEDA")]
    public string Moeda { get; set; } = "BRL";
    [Column("DATA_COMPRA")]
    public DateTime DataCompra { get; set; }
    [Column("DESCRICAO")]
    public string? Descricao { get; set; }
    public Usuario? Usuario { get; set; }
    public Categoria? Categoria { get; set; }
}
