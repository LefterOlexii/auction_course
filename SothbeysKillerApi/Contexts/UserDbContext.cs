using Microsoft.EntityFrameworkCore;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Contexts;

public interface IUserDbContext
{
    DbSet<User> Type { get; set; }
    int SaveChanges();
}

public class UserDbContext : DbContext, IUserDbContext
{
    public UserDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
    {
    }
    public DbSet<User> Type { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Name)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Password)
            .IsRequired();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        base.OnModelCreating(modelBuilder);
    }
}