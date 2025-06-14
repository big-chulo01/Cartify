// Data/SecurityDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class SecurityDbContext : IdentityDbContext
{
    public SecurityDbContext(DbContextOptions<SecurityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}

