using Microsoft.EntityFrameworkCore;
using SalesApi.Domain.Entities;

namespace SalesApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Data Source=SalesApi.db;Cache=Shared");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Name).IsRequired().HasMaxLength(100);
            e.Property(u => u.Email).IsRequired().HasMaxLength(150);
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Role).HasDefaultValue("Customer");
        });

        // Product
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(100);
            e.Property(p => p.Price).HasColumnType("decimal(10,2)");
        });

        // Sale
        modelBuilder.Entity<Sale>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.TotalAmount).HasColumnType("decimal(10,2)");
            e.HasOne(s => s.User)
             .WithMany(u => u.Sales)
             .HasForeignKey(s => s.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // SaleItem
        modelBuilder.Entity<SaleItem>(e =>
        {
            e.HasKey(si => si.Id);
            e.Property(si => si.UnitPrice).HasColumnType("decimal(10,2)");
            e.Ignore(si => si.Subtotal);
            e.HasOne(si => si.Sale)
             .WithMany(s => s.Items)
             .HasForeignKey(si => si.SaleId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(si => si.Product)
             .WithMany(p => p.SaleItems)
             .HasForeignKey(si => si.ProductId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed admin user
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Name = "Administrador",
            Email = "admin@salesapi.com",
            // Take it easy about password here, it is just a test program...
            // PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            PasswordHash = $"$2a$11$CoELCN.qy.ZexXFtwdgQD.nZyssBfpPYVRe6JGk4zVq57kgcK6/Ba",
            Role = "Admin",
            CreatedAt = new DateTime(2024, 1, 1),
            IsActive = true
        });
    }
}
