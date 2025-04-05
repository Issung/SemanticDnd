using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Mre.Data;
using Mre.Data.Model;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System.Diagnostics;

namespace Mre;

/// <summary>
/// This program is a minimal reproducible example of some sort of bug in either Hangfire.PostgreSql or Pgvector.EntityFrameworkCore.
/// 
/// STEPS:
/// 
/// 1. Create a pgvector docker container with this command:
/// docker run -d --name mre -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=password -e POSTGRES_DB=mre -p 20592:5432 pgvector/pgvector:pg17
/// 
/// 2. Run `dotnet ef database update` to apply the migration.
/// 
/// 3. Run this code. Once with `app.UseHangfireDashboard()` included, and once with it removed.
/// </summary>
/// <remarks>
/// Initial migration created with `dotnet ef migrations add InitialMigration --output-dir Data/Migrations`.
/// </remarks>
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 100_000_000; // Set the desired size (bytes)
        });

        builder.Logging.SetMinimumLevel(LogLevel.Trace);

        builder.Services.AddHangfire(config =>
        {
            config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(TestDbContext.ConnectionString));
        });

        builder.Services.AddHangfireServer(config =>
        {

        });

        builder.Services.AddDbContext<TestDbContext>();

        var app = builder.Build();

        // Comment this out to fix the program !!!
        //app.UseHangfireDashboard();
        // Comment this out to fix the program !!!

        // Test
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var randomString = new string(Enumerable.Range(0, 32).Select(_ => chars[Random.Shared.Next(chars.Length)]).ToArray());
        var floats = Enumerable.Range(0, 768).Select(_ => (float)Random.Shared.NextDouble()).ToArray();

        var ec = new TestModel() { Vector = new Vector(floats) };

        dbContext.Models.Add(ec);

        // If `app.UseHangfireDashboard()` is run this SaveChangesAsync line will throw an exception:
        // `InvalidCastException: Writing values of 'Pgvector.Vector' is not supported for parameters having no NpgsqlDbType or DataTypeName. Try setting one of these values to the expected database type..`
        dbContext.SaveChanges();

        var results = dbContext.Models.OrderBy(ec => ec.Vector.L2Distance(new Vector(floats))).ToList();

        Debug.Assert(results.Any(r => r.Id == ec.Id));

        return;
    }
}
