using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Stockas.Entities;

public partial class StockasContext : DbContext
{
    public StockasContext()
    {
    }

    public StockasContext(DbContextOptions<StockasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionCategory> TransactionCategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=tcp:stockas.database.windows.net,1433;Database=stockas;User ID=GroupAdmin7;Password=Group_Admin07;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.InputDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ProductName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SellingPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_ProductCategory");

            entity.HasOne(d => d.User).WithMany(p => p.Products)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Users");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.ToTable("ProductCategory");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductCategory_Users");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TransactionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_TransactionCategory");

            entity.HasOne(d => d.Product).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_Transactions_Product");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transactions_Users");
        });

        modelBuilder.Entity<TransactionCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId);

            entity.ToTable("TransactionCategory");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.TransactionCategories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionCategory_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_User");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053411BB65F1").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.RefreshTokenExpiry).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}