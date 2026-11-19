using Microsoft.EntityFrameworkCore;
using VaciSmart.Api.Models;

namespace VaciSmart.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UnidadeSaude> Unidades => Set<UnidadeSaude>();
    public DbSet<Transporte> Transportes => Set<Transporte>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UnidadeSaude>()
            .HasKey(u => u.Id)
            .HasName("PK_VS_UNIDADE_SAUDE");

        modelBuilder.Entity<Transporte>(transporte =>
        {
            transporte.HasKey(t => t.Id).HasName("PK_VS_TRANSPORTE");

            // Dois transportes não podem ter o mesmo código.
            transporte.HasIndex(t => t.Codigo)
                .IsUnique()
                .HasDatabaseName("UK_VS_TRANSPORTE_CODIGO");

            transporte.HasIndex(t => t.UnidadeDestinoId)
                .HasDatabaseName("IX_VS_TRANSPORTE_UNIDADE");

            // Relacionamento 1:N — UnidadeSaude (1) -> Transporte (N).
            // Restrict: o banco não deixa excluir uma unidade que ainda tem transportes.
            transporte.HasOne(t => t.UnidadeDestino)
                .WithMany(u => u.Transportes)
                .HasForeignKey(t => t.UnidadeDestinoId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_VS_TRANSPORTE_UNIDADE");
        });
    }
}
