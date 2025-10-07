using DuckBill.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DuckBill.Infrastructure;

public class DuckBillDbContext : DbContext
{
    public DuckBillDbContext(DbContextOptions<DuckBillDbContext> options) : base(options) {}

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Despesa> Despesas => Set<Despesa>();
    public DbSet<Ativo> Ativos => Set<Ativo>();
    public DbSet<TransacaoAtivo> TransacoesAtivo => Set<TransacaoAtivo>();
    public DbSet<CotacaoAtivo> CotacoesAtivo => Set<CotacaoAtivo>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<Usuario>(e =>
        {
            e.ToTable("USUARIO");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(120).IsRequired();
            e.Property(x => x.Senha).HasMaxLength(64).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
        });

        mb.Entity<Categoria>(e =>
        {
            e.ToTable("CATEGORIA");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasMaxLength(60).IsRequired();
            e.HasIndex(x => x.Nome).IsUnique();
        });

        mb.Entity<Despesa>(e =>
        {
            e.ToTable("DESPESA");
            e.HasKey(x => x.Id);
            e.Property(x => x.Valor).HasColumnType("NUMBER(12,2)");
            e.Property(x => x.Moeda).HasMaxLength(3).HasDefaultValue("BRL");
            e.Property(x => x.Descricao).HasMaxLength(200);
            e.Property(x => x.DataCompra).HasColumnType("DATE");
            e.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId);
            e.HasOne(x => x.Categoria).WithMany(c => c.Despesas).HasForeignKey(x => x.CategoriaId);
        });

        mb.Entity<Ativo>(e =>
        {
            e.ToTable("ATIVO");
            e.HasKey(x => x.Id);
            e.Property(x => x.Ticker).HasMaxLength(12).IsRequired();
            e.Property(x => x.Tipo).HasMaxLength(20).IsRequired();
            e.Property(x => x.MoedaBase).HasMaxLength(3).HasDefaultValue("BRL");
            e.HasIndex(x => x.Ticker).IsUnique();
        });

        mb.Entity<TransacaoAtivo>(e =>
        {
            e.ToTable("TRANSACAO_ATIVO");
            e.HasKey(x => x.Id);
            e.Property(x => x.Tipo).HasMaxLength(4).IsRequired();
            e.Property(x => x.Qtd).HasColumnType("NUMBER(12,4)");
            e.Property(x => x.Preco).HasColumnType("NUMBER(14,6)");
            e.Property(x => x.DataNegocio).HasColumnType("DATE");
            e.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId);
            e.HasOne(x => x.Ativo).WithMany(a => a.Transacoes).HasForeignKey(x => x.AtivoId);
        });

        mb.Entity<CotacaoAtivo>(e =>
        {
            e.ToTable("COTACAO_ATIVO");
            e.HasKey(x => new { x.AtivoId, x.DataRef });
            e.Property(x => x.PrecoFech).HasColumnType("NUMBER(14,6)");
            e.Property(x => x.DataRef).HasColumnType("DATE");
            e.HasOne(x => x.Ativo).WithMany(a => a.Cotacoes).HasForeignKey(x => x.AtivoId);
        });
    }
}
