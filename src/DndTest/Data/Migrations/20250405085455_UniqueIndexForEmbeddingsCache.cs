using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DndTest.Data.Migrations
{
    /// <inheritdoc />
    public partial class UniqueIndexForEmbeddingsCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmbeddingCache_TextHash_Model",
                table: "EmbeddingCache");

            migrationBuilder.CreateIndex(
                name: "IX_EmbeddingCache_TextHash_Model",
                table: "EmbeddingCache",
                columns: new[] { "TextHash", "Model" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmbeddingCache_TextHash_Model",
                table: "EmbeddingCache");

            migrationBuilder.CreateIndex(
                name: "IX_EmbeddingCache_TextHash_Model",
                table: "EmbeddingCache",
                columns: new[] { "TextHash", "Model" });
        }
    }
}
