using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DndTest.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomFieldTenantRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "CustomFields",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CustomFields_TenantId",
                table: "CustomFields",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomFields_Tenants_TenantId",
                table: "CustomFields",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomFields_Tenants_TenantId",
                table: "CustomFields");

            migrationBuilder.DropIndex(
                name: "IX_CustomFields_TenantId",
                table: "CustomFields");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CustomFields");
        }
    }
}
