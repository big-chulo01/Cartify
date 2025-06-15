using CartifyApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace CartifyApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Cartify> Cartifies { get; set; }
}