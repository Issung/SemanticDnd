using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DndTest.Data.Migrations
{
    /// <inheritdoc />
    public partial class SearchChunkRelatesToItemNotNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchChunks_Notes_DocumentId",
                table: "SearchChunks");

            migrationBuilder.RenameColumn(
                name: "DocumentId",
                table: "SearchChunks",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_SearchChunks_DocumentId",
                table: "SearchChunks",
                newName: "IX_SearchChunks_ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchChunks_Items_ItemId",
                table: "SearchChunks",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchChunks_Items_ItemId",
                table: "SearchChunks");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "SearchChunks",
                newName: "DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_SearchChunks_ItemId",
                table: "SearchChunks",
                newName: "IX_SearchChunks_DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchChunks_Notes_DocumentId",
                table: "SearchChunks",
                column: "DocumentId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
