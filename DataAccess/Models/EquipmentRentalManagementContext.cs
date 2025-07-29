using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<RentalContract> RentalContracts { get; set; }

    public virtual DbSet<RentalDetail> RentalDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("DB"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("PK__Equipmen__34474599ED32704F");

            entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.DailyRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A58D48CC2BD");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.Note).HasMaxLength(255);

            entity.HasOne(d => d.Contract).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK__Payment__Contrac__571DF1D5");
        });

        modelBuilder.Entity<RentalContract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__RentalCo__C90D34099267878E");

            entity.ToTable("RentalContract");

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.RentalContracts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RentalCon__UserI__5070F446");
        });

        modelBuilder.Entity<RentalDetail>(entity =>
        {
            entity.HasKey(e => e.RentalDetailId).HasName("PK__RentalDe__10B35999278E82E2");

            entity.ToTable("RentalDetail");

            entity.Property(e => e.RentalDetailId).HasColumnName("RentalDetailID");
            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");
            entity.Property(e => e.RatePerDay).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Contract).WithMany(p => p.RentalDetails)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK__RentalDet__Contr__534D60F1");

            entity.HasOne(d => d.Equipment).WithMany(p => p.RentalDetails)
                .HasForeignKey(d => d.EquipmentId)
                .HasConstraintName("FK__RentalDet__Equip__5441852A");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4CE4222063");

            entity.ToTable("User");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(25);
            entity.Property(e => e.Phone).HasMaxLength(10);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Staff");
            entity.Property(e => e.Username).HasMaxLength(25);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
