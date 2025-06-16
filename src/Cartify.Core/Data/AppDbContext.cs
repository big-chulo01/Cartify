using Cartify.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cartify.Core.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
            
            // Relationship with Category
            entity.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(200);
        });

        // ShoppingCart configuration
        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(sc => sc.Id);
            entity.Property(sc => sc.User)
                .IsRequired()
                .HasMaxLength(256); // Matches IdentityUser email length
            
            // Relationship with CartItems
            entity.HasMany(sc => sc.Items)
                .WithOne()
                .HasForeignKey(ci => ci.ShoppingCartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CartItem configuration
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(ci => ci.Id);
            
            entity.Property(ci => ci.Quantity)
                .IsRequired()
                .HasDefaultValue(1);
            
            entity.Property(ci => ci.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            // Relationship with Product
            entity.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}