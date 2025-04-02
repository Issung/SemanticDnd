using Microsoft.EntityFrameworkCore;
using VectorSimple.Data.Model;

namespace VectorSimple.Data;

public class MyDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<TestModel> TestModels { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresExtension("vector");

        builder
            .Entity<TestModel>(tm =>
            {
                tm.HasIndex(e => e.Embedding)
                    .HasMethod("ivfflat")
                    .HasOperators("vector_l2_ops")
                    .HasStorageParameter("lists", 100);
            });

        base.OnModelCreating(builder);
    }
}
