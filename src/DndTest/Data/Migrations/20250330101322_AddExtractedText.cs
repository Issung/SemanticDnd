using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DndTest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExtractedText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchChunks_EmbeddingCache_EmbeddingId",
                table: "SearchChunks");

            migrationBuilder.DropIndex(
                name: "IX_SearchChunks_EmbeddingId",
                table: "SearchChunks");

            migrationBuilder.DropColumn(
                name: "EmbeddingId",
                table: "SearchChunks");

            migrationBuilder.AddColumn<int>(
                name: "PageNumber",
                table: "SearchChunks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExtractedText",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    PageNumber = table.Column<int>(type: "integer", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtractedText", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtractedText_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExtractedText_FileId",
                table: "ExtractedText",
                column: "FileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExtractedText");

            migrationBuilder.DropColumn(
                name: "PageNumber",
                table: "SearchChunks");

            migrationBuilder.AddColumn<int>(
                name: "EmbeddingId",
                table: "SearchChunks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SearchChunks_EmbeddingId",
                table: "SearchChunks",
                column: "EmbeddingId");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchChunks_EmbeddingCache_EmbeddingId",
                table: "SearchChunks",
                column: "EmbeddingId",
                principalTable: "EmbeddingCache",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
