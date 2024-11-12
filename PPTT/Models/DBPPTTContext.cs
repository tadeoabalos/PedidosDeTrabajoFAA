using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using static PPTT.Pages.PT.DetailsPPTT;
namespace PPTT.Models;

public partial class DBPPTTContext : DbContext
{
    public DBPPTTContext()
    {
    }

    public DBPPTTContext(DbContextOptions<DBPPTTContext> options)
        : base(options)
    {
    }

    // Add the DbSet for CorreoResult
    public DbSet<Division> Division { get; set; }

    public virtual DbSet<Prueba> Pruebas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionSQL");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Prueba>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Prueba");

            entity.Property(e => e.ID).HasColumnName("ID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
