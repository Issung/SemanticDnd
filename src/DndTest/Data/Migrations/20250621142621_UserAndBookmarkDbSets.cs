using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DndTest.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserAndBookmarkDbSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookmark_BookmarkCollection_BookmarkCollectionId",
                table: "Bookmark");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmark_Items_ItemId",
                table: "Bookmark");

            migrationBuilder.DropForeignKey(
                name: "FK_BookmarkCollection_User_UserId",
                table: "BookmarkCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Tenants_TenantId",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookmarkCollection",
                table: "BookmarkCollection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookmark",
                table: "Bookmark");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "BookmarkCollection",
                newName: "BookmarkCollections");

            migrationBuilder.RenameTable(
                name: "Bookmark",
                newName: "Bookmarks");

            migrationBuilder.RenameIndex(
                name: "IX_User_TenantId",
                table: "Users",
                newName: "IX_Users_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_BookmarkCollection_UserId",
                table: "BookmarkCollections",
                newName: "IX_BookmarkCollections_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookmark_ItemId",
                table: "Bookmarks",
                newName: "IX_Bookmarks_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookmarkCollections",
                table: "BookmarkCollections",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookmarks",
                table: "Bookmarks",
                columns: new[] { "BookmarkCollectionId", "ItemId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookmarkCollections_Users_UserId",
                table: "BookmarkCollections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_BookmarkCollections_BookmarkCollectionId",
                table: "Bookmarks",
                column: "BookmarkCollectionId",
                principalTable: "BookmarkCollections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmarks_Items_ItemId",
                table: "Bookmarks",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookmarkCollections_Users_UserId",
                table: "BookmarkCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_BookmarkCollections_BookmarkCollectionId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookmarks_Items_ItemId",
                table: "Bookmarks");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookmarks",
                table: "Bookmarks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookmarkCollections",
                table: "BookmarkCollections");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "Bookmarks",
                newName: "Bookmark");

            migrationBuilder.RenameTable(
                name: "BookmarkCollections",
                newName: "BookmarkCollection");

            migrationBuilder.RenameIndex(
                name: "IX_Users_TenantId",
                table: "User",
                newName: "IX_User_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookmarks_ItemId",
                table: "Bookmark",
                newName: "IX_Bookmark_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_BookmarkCollections_UserId",
                table: "BookmarkCollection",
                newName: "IX_BookmarkCollection_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookmark",
                table: "Bookmark",
                columns: new[] { "BookmarkCollectionId", "ItemId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookmarkCollection",
                table: "BookmarkCollection",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmark_BookmarkCollection_BookmarkCollectionId",
                table: "Bookmark",
                column: "BookmarkCollectionId",
                principalTable: "BookmarkCollection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookmark_Items_ItemId",
                table: "Bookmark",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookmarkCollection_User_UserId",
                table: "BookmarkCollection",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Tenants_TenantId",
                table: "User",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
