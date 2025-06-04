using DndTest.Data.Model;
using DndTest.Data.Model.Content;
using DndTest.Data.Model.CustomFields;
using DndTest.Helpers.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DndTest.Data;

public class DndDbContext(
    DbContextOptions<DndDbContext> options
) : IdentityDbContext(options)
{
    public DbSet<Tenant> Tenants { get; set; } = null!;

    // Content
    public DbSet<Item> Items { get; set; } = null!;
    public DbSet<Model.Content.File> Files { get; set; } = default!;
    public DbSet<Folder> Folders { get; set; } = default!;
    public DbSet<Note> Notes { get; set; } = default!;
    public DbSet<Shortcut> Shortcuts { get; set; } = default!;
    //public IQueryable<Model.Content.File> Files => Items.OfType<Model.Content.File>();
    //public IQueryable<Folder> Folders => Items.OfType<Folder>();
    //public IQueryable<Note> Notes => Items.OfType<Note>();
    //public IQueryable<Shortcut> Shortcuts => Items.OfType<Shortcut>();

    public DbSet<CustomField> CustomFields { get; set; } = null!;

    public DbSet<EmbeddingCache> EmbeddingsCache { get; set; } = null!;
    public DbSet<TikaCache> TikaCache { get; set; } = null!;
    public DbSet<ExtractedText> ExtractedText { get; set; }
    public DbSet<SearchChunk> SearchChunks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var cs = "Host=localhost;Port=5432;Database=dndtest;Username=postgres;Password=password";

        optionsBuilder.UseNpgsql(cs, o => o.UseVector());

        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresExtension("vector");

        builder
            .Entity<SearchChunk>(searchChunk =>
            {
                searchChunk.Property(d => d.TextVector).HasComputedColumnSql("to_tsvector('english', \"SearchChunks\".\"Text\")", stored: true);
                searchChunk.HasIndex(s => s.TextVector).HasMethod("GIN");

                searchChunk.HasIndex(e => e.EmbeddingVector)
                    .HasMethod("ivfflat")
                    .HasOperators("vector_l2_ops")
                    .HasStorageParameter("lists", 100);
            });

        builder.Entity<CustomFieldCondition>(c =>
        {
            c.HasOne(c => c.CustomField)
                .WithMany(cf => cf.Conditions)
                .HasForeignKey(c => c.CustomFieldId)
                .OnDelete(DeleteBehavior.Cascade);

            c.HasOne(c => c.DependsOnCustomField)
                .WithMany(cf => cf.DependentConditions)
                .HasForeignKey(cfc => cfc.DependsOnCustomFieldId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Item>().UseTptMappingStrategy();

        base.OnModelCreating(builder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder
            .Properties<Enum>()
            .HaveConversion<string>()
            .HaveColumnType("VARCHAR(64)"); // Hopefully no enum longer than this.
    }

    public override void Dispose()
    {
        base.Dispose();
    }

    public override ValueTask DisposeAsync()
    {
        return base.DisposeAsync();
    }
}
