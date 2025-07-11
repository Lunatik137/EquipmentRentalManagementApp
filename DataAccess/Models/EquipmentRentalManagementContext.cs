using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

public partial class EquipmentRentalManagementContext : DbContext
{
    public EquipmentRentalManagementContext()
    {
    }

    public EquipmentRentalManagementContext(DbContextOptions<EquipmentRentalManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<RentalContract> RentalContracts { get; set; }

    public virtual DbSet<RentalDetail> RentalDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=EquipmentRentalManagement;UId=sa;pwd=123;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("PK__Equipmen__3447459953A74562");

            entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.DailyRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.HasKey(e => e.OwnerId).HasName("PK__Owner__81938598EFE32C92");

            entity.ToTable("Owner");

            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(25);
            entity.Property(e => e.Phone).HasMaxLength(10);
            entity.Property(e => e.Username).HasMaxLength(25);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A58AF311435");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.Note).HasMaxLength(255);

            entity.HasOne(d => d.Contract).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK__Payment__Contrac__5629CD9C");
        });

        modelBuilder.Entity<RentalContract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__RentalCo__C90D34094E50ECDC");

            entity.ToTable("RentalContract");

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.OwnerId).HasColumnName("OwnerID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Owner).WithMany(p => p.RentalContracts)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("FK__RentalCon__Owner__4F7CD00D");
        });

        modelBuilder.Entity<RentalDetail>(entity =>
        {
            entity.HasKey(e => e.RentalDetailId).HasName("PK__RentalDe__10B359991AD2E030");

            entity.ToTable("RentalDetail");

            entity.Property(e => e.RentalDetailId).HasColumnName("RentalDetailID");
            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");
            entity.Property(e => e.RatePerDay).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Contract).WithMany(p => p.RentalDetails)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK__RentalDet__Contr__52593CB8");

            entity.HasOne(d => d.Equipment).WithMany(p => p.RentalDetails)
                .HasForeignKey(d => d.EquipmentId)
                .HasConstraintName("FK__RentalDet__Equip__534D60F1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
