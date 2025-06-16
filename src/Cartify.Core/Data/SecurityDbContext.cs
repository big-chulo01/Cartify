using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cartify.Core.Data;

public class SecurityDbContext : IdentityDbContext
{
    public SecurityDbContext(DbContextOptions<SecurityDbContext> options) 
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Customize Identity tables if needed
        builder.Entity<IdentityUser>(entity => 
        {
            entity.Property(u => u.Email)
                .HasMaxLength(256);
            
            entity.Property(u => u.NormalizedEmail)
                .HasMaxLength(256);
            
            entity.Property(u => u.UserName)
                .HasMaxLength(256);
            
            entity.Property(u => u.NormalizedUserName)
                .HasMaxLength(256);
        });
        
        // Shorten identity column lengths for SQL Server
        builder.Entity<IdentityRole>(entity => 
        {
            entity.Property(r => r.Name)
                .HasMaxLength(128);
            
            entity.Property(r => r.NormalizedName)
                .HasMaxLength(128);
        });
        
        builder.Entity<IdentityUserToken<string>>(entity => 
        {
            entity.Property(t => t.LoginProvider)
                .HasMaxLength(128);
            
            entity.Property(t => t.Name)
                .HasMaxLength(128);
        });
    }
}