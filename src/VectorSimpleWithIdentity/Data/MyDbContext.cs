using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VectorSimpleWithIdentity.Data.Model;

namespace VectorSimpleWithIdentity.Data;

public class MyDbContext(DbContextOptions options) : IdentityDbContext(options)
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
