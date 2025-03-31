using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DndTest.Data.Migrations
{
    /// <inheritdoc />
    public partial class TikaCacheAndAddModelToEmbeddingsCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchChunks_CachedEmbeddings_EmbeddingId",
                table: "SearchChunks");

            migrationBuilder.DropTable(
                name: "CachedEmbeddings");

            migrationBuilder.CreateTable(
                name: "EmbeddingCache",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: false),
                    TextHash = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Floats = table.Column<float[]>(type: "real[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbeddingCache", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TikaCache",
                columns: table => new
                {
                    FileHash = table.Column<string>(type: "text", nullable: false),
                    TikaResponseJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TikaCache", x => x.FileHash);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmbeddingCache_TextHash_Model",
                table: "EmbeddingCache",
                columns: new[] { "TextHash", "Model" });

            migrationBuilder.AddForeignKey(
                name: "FK_SearchChunks_EmbeddingCache_EmbeddingId",
                table: "SearchChunks",
                column: "EmbeddingId",
                principalTable: "EmbeddingCache",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchChunks_EmbeddingCache_EmbeddingId",
                table: "SearchChunks");

            migrationBuilder.DropTable(
                name: "EmbeddingCache");

            migrationBuilder.DropTable(
                name: "TikaCache");

            migrationBuilder.CreateTable(
                name: "CachedEmbeddings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Floats = table.Column<float[]>(type: "real[]", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    TextHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachedEmbeddings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CachedEmbeddings_TextHash",
                table: "CachedEmbeddings",
                column: "TextHash");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchChunks_CachedEmbeddings_EmbeddingId",
                table: "SearchChunks",
                column: "EmbeddingId",
                principalTable: "CachedEmbeddings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
