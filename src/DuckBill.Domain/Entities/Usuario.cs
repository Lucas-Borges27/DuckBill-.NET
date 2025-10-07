using System.ComponentModel.DataAnnotations.Schema;

namespace DuckBill.Domain.Entities;

public class Usuario
{
    [Column("ID")]
    public long Id { get; set; }
    [Column("NOME")]
    public string Nome { get; set; } = default!;
    [Column("EMAIL")]
    public string Email { get; set; } = default!;
    [Column("SENHA")]
    public string Senha { get; set; } = default!;
}
