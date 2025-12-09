using Microsoft.EntityFrameworkCore;
using MissSolitude.Domain;
using MissSolitude.Domain.Entities;
using MissSolitude.Domain.ValueObjects;

namespace MissSolitude.Infrastructure;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builderUser =>
        {
            builderUser.Property(user => user.Email)
                .HasConversion(email => email.Value, value => new EmailAddress(value)).HasColumnName("Email");
        });
    }

    public DbSet<User> Users { get; set; }
}