using System.ComponentModel.DataAnnotations.Schema;

namespace DuckBill.Domain.Entities;

public class Categoria
{
    [Column("ID")]
    public long Id { get; set; }
    [Column("NOME")]
    public string Nome { get; set; } = default!;
    public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
}
