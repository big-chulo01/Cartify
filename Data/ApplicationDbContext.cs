// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships if needed
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId);
    }
}