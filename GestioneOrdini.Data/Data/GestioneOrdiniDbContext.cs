using GestioneOrdini.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GestioneOrdini.Data.Data;

public sealed class GestioneOrdiniDbContext : DbContext
{
    public GestioneOrdiniDbContext(
        DbContextOptions<GestioneOrdiniDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clienti => Set<Cliente>();
    public DbSet<Prodotto> Prodotti => Set<Prodotto>();
    public DbSet<Ordine> Ordini => Set<Ordine>();
    public DbSet<RigaOrdine> RigheOrdine => Set<RigaOrdine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clienti");
            entity.HasKey(cliente => cliente.IdCliente);
            entity.Property(cliente => cliente.Nome).HasMaxLength(100);
            entity.Property(cliente => cliente.Email).HasMaxLength(254);
            entity.HasIndex(cliente => cliente.Email).IsUnique();
        });

        modelBuilder.Entity<Prodotto>(entity =>
        {
            entity.ToTable("Prodotti");
            entity.HasKey(prodotto => prodotto.IdProdotto);
            entity.Property(prodotto => prodotto.Nome).HasMaxLength(150);
            entity.Property(prodotto => prodotto.Prezzo).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Ordine>(entity =>
        {
            entity.ToTable("Ordini");
            entity.HasKey(ordine => ordine.IdOrdine);
            entity.Property(ordine => ordine.DataOrdine)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(ordine => ordine.Cliente)
                .WithMany()
                .HasForeignKey(ordine => ordine.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RigaOrdine>(entity =>
        {
            entity.ToTable("RigheOrdine");
            entity.HasKey(riga => new { riga.IdOrdine, riga.IdProdotto });
            entity.Property(riga => riga.PrezzoUnitario).HasPrecision(10, 2);
            entity.HasOne(riga => riga.Ordine)
                .WithMany(ordine => ordine.Righe)
                .HasForeignKey(riga => riga.IdOrdine)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(riga => riga.Prodotto)
                .WithMany()
                .HasForeignKey(riga => riga.IdProdotto)
                .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}
