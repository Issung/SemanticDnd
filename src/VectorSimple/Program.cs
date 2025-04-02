using Microsoft.EntityFrameworkCore;
using VectorSimple.Data;
using VectorSimple.Data.Model;

namespace VectorSimple;

/// <summary>
/// docker run -d --name pgtest1 -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=password -e POSTGRES_DB=mytestdatabase -p 5432:5432 pgvector/pgvector:pg17
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        builder.Services.AddDbContext<MyDbContext>(options =>
        {
            const string connectionString = "Host=localhost;Port=5432;Database=mytestdatabase;Username=postgres;Password=password";
            options.UseNpgsql(connectionString, o => o.UseVector());
            options.EnableSensitiveDataLogging();
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.UseAuthorization();

        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        var floats = Enumerable.Range(0, 768).Select(_ => (float)Random.Shared.NextDouble()).ToArray();

        var model = new TestModel() { Embedding = new Pgvector.Vector(floats) };

        dbContext.TestModels.Add(model);

        await dbContext.SaveChangesAsync();

        //app.Run();
    }
}
