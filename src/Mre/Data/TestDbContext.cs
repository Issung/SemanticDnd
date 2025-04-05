using Microsoft.EntityFrameworkCore;
using Mre.Data.Model;

namespace Mre.Data;

public class TestDbContext : DbContext
{
    public DbSet<TestModel> Models { get; set; } = null!;

    public const string ConnectionString = "Host=localhost;Port=20592;Database=mre;Username=postgres;Password=password";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseNpgsql(ConnectionString, o => o.UseVector());
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresExtension("vector");

        base.OnModelCreating(builder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }
}
