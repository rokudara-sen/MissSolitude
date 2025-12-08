using Microsoft.EntityFrameworkCore;
using MissSolitude.Domain;

namespace MissSolitude.Infrastructure;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
}