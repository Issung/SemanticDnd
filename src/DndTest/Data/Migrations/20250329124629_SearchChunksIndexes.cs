using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DndTest.Data.Migrations
{
    /// <inheritdoc />
    public partial class SearchChunksIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SearchChunks_EmbeddingVector",
                table: "SearchChunks",
                column: "EmbeddingVector")
                .Annotation("Npgsql:IndexMethod", "ivfflat")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_l2_ops" })
                .Annotation("Npgsql:StorageParameter:lists", 100);

            migrationBuilder.CreateIndex(
                name: "IX_SearchChunks_TextVector",
                table: "SearchChunks",
                column: "TextVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SearchChunks_EmbeddingVector",
                table: "SearchChunks");

            migrationBuilder.DropIndex(
                name: "IX_SearchChunks_TextVector",
                table: "SearchChunks");
        }
    }
}
