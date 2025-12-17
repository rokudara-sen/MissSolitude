using Microsoft.EntityFrameworkCore;
using MissSolitude.Infrastructure;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Fixtures;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .Build();

    public DatabaseContext Context { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        // 1. Start the Container
        await _postgres.StartAsync();

        // 2. Setup DB Context Options
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        // 3. Create the Context
        Context = new DatabaseContext(options);

        // 4. Create Tables
        await Context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    // Helper to clean data between tests if necessary
    public async Task ResetDatabaseAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        await Context.SaveChangesAsync();
    }
}