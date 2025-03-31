using DndTest.Config;
using DndTest.Data.Migrations;
using DndTest.Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DndTest.Data;

public class DndDbContext(
    DbContextOptions<DndDbContext> options,
    DndSettings settings
) : IdentityDbContext(options)
{
    public DbSet<Document> Documents { get; set; } = null!;
    public DbSet<EmbeddingCache> EmbeddingsCache { get; set; } = null!;
    public DbSet<TikaCache> TikaCache { get; set; } = null!;
    public DbSet<Model.File> Files { get; set; } = null!;
    public DbSet<ExtractedText> ExtractedText { get; set; }
    public DbSet<SearchChunk> SearchChunks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("vector");

        modelBuilder
            .Entity<SearchChunk>(searchChunk =>
            {
                searchChunk.Property(d => d.TextVector).HasComputedColumnSql("to_tsvector('english', \"SearchChunks\".\"Text\")", stored: true);
                searchChunk.HasIndex(s => s.TextVector).HasMethod("GIN");

                searchChunk
                    .Property(d => d.EmbeddingVector)
                    .HasColumnType($"vector({settings.EmbeddingsSize})")
                    //.HasConversion(
                    //    v => string.Join(',', v),
                    //    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    //          .Select(float.Parse).ToArray()
                    //)
                ;

                searchChunk.HasIndex(e => e.EmbeddingVector)
                    .HasMethod("ivfflat")
                    .HasOperators("vector_l2_ops")
                    .HasStorageParameter("lists", 100);
            });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder
            .Properties<Enum>()
            .HaveConversion<string>();
    }

}
