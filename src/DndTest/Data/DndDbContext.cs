using DndTest.Config;
using DndTest.Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DndTest.Data;

public class DndDbContext(
    DbContextOptions<DndDbContext> options
) : IdentityDbContext(options)
{
    public DbSet<Document> Documents { get; set; } = null!;
    public DbSet<EmbeddingCache> EmbeddingsCache { get; set; } = null!;
    public DbSet<TikaCache> TikaCache { get; set; } = null!;
    public DbSet<Model.File> Files { get; set; } = null!;
    public DbSet<ExtractedText> ExtractedText { get; set; }
    public DbSet<SearchChunk> SearchChunks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresExtension("vector");

        builder
            .Entity<EmbeddingCache>(builder =>
            {
                builder.Property(ec => ec.Vector).HasColumnType("vector(768)");
            });

        builder
            .Entity<SearchChunk>(searchChunk =>
            {
                searchChunk.Property(d => d.TextVector).HasComputedColumnSql("to_tsvector('english', \"SearchChunks\".\"Text\")", stored: true);
                searchChunk.HasIndex(s => s.TextVector).HasMethod("GIN");

                searchChunk.Property(sc => sc.EmbeddingVector).HasColumnType("vector(768)");
                searchChunk.HasIndex(e => e.EmbeddingVector)
                    .HasMethod("ivfflat")
                    .HasOperators("vector_l2_ops")
                    .HasStorageParameter("lists", 100);
            });

        base.OnModelCreating(builder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder
            .Properties<Enum>()
            .HaveConversion<string>();
    }

}
