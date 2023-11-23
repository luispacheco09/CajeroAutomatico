using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DL;

public partial class CajeroAutomaticoContext : DbContext
{
    public CajeroAutomaticoContext()
    {
    }

    public CajeroAutomaticoContext(DbContextOptions<CajeroAutomaticoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Atm> Atms { get; set; }

    public virtual DbSet<Atmdenominacion> Atmdenominacions { get; set; }

    public virtual DbSet<Cuentum> Cuenta { get; set; }

    public virtual DbSet<Denominacion> Denominacions { get; set; }

    public virtual DbSet<Movimiento> Movimientos { get; set; }

    public virtual DbSet<MovimientoDenominacion> MovimientoDenominacions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.; Database= CajeroAutomatico; TrustServerCertificate=True; Trusted_Connection=True; User ID=sa; Password=pass@word1;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atm>(entity =>
        {
            entity.HasKey(e => e.IdAtm).HasName("PK__ATM__0E242A713CCEC372");

            entity.ToTable("ATM");

            entity.Property(e => e.IdAtm).HasColumnName("IdATM");
            entity.Property(e => e.Saldo).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Atmdenominacion>(entity =>
        {
            entity.HasKey(e => e.IdAtmdenominacion).HasName("PK__ATMDenom__681AFB0DE8AB4DF9");

            entity.ToTable("ATMDenominacion");

            entity.Property(e => e.IdAtmdenominacion).HasColumnName("IdATMDenominacion");
            entity.Property(e => e.IdAtm).HasColumnName("IdATM");

            entity.HasOne(d => d.IdAtmNavigation).WithMany(p => p.Atmdenominacions)
                .HasForeignKey(d => d.IdAtm)
                .HasConstraintName("FK__ATMDenomi__IdATM__145C0A3F");

            entity.HasOne(d => d.IdDenominacionNavigation).WithMany(p => p.Atmdenominacions)
                .HasForeignKey(d => d.IdDenominacion)
                .HasConstraintName("FK__ATMDenomi__IdDen__15502E78");
        });

        modelBuilder.Entity<Cuentum>(entity =>
        {
            entity.HasKey(e => e.IdCuenta).HasName("PK__Cuenta__D41FD706E8C276C4");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Saldo).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Denominacion>(entity =>
        {
            entity.HasKey(e => e.IdDenominacion).HasName("PK__Denomina__32C0B9145D1072A6");

            entity.ToTable("Denominacion");
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("PK__Movimien__881A6AE0F764F2BB");

            entity.ToTable("Movimiento");

            entity.Property(e => e.FechaMovimiento).HasColumnType("date");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.IdCuenta)
                .HasConstraintName("FK__Movimient__IdCue__1A14E395");
        });

        modelBuilder.Entity<MovimientoDenominacion>(entity =>
        {
            entity.HasKey(e => e.IdMovimientoDenominacion).HasName("PK__Movimien__F612881DE88B3936");

            entity.ToTable("MovimientoDenominacion");

            entity.HasOne(d => d.IdDenominacionNavigation).WithMany(p => p.MovimientoDenominacions)
                .HasForeignKey(d => d.IdDenominacion)
                .HasConstraintName("FK__Movimient__IdDen__1DE57479");

            entity.HasOne(d => d.IdMovimientoNavigation).WithMany(p => p.MovimientoDenominacions)
                .HasForeignKey(d => d.IdMovimiento)
                .HasConstraintName("FK__Movimient__IdMov__1CF15040");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
