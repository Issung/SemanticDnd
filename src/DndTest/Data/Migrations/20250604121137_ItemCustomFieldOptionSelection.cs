using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DndTest.Migrations
{
    /// <inheritdoc />
    public partial class ItemCustomFieldOptionSelection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomFieldOption_ItemCustomFieldValue_ItemCustomFieldValue~",
                table: "CustomFieldOption");

            migrationBuilder.DropIndex(
                name: "IX_CustomFieldOption_ItemCustomFieldValueId",
                table: "CustomFieldOption");

            migrationBuilder.DropColumn(
                name: "ItemCustomFieldValueId",
                table: "CustomFieldOption");

            migrationBuilder.CreateTable(
                name: "CustomFieldOptionItemCustomFieldValue",
                columns: table => new
                {
                    SelectedOnId = table.Column<int>(type: "integer", nullable: false),
                    ValuesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFieldOptionItemCustomFieldValue", x => new { x.SelectedOnId, x.ValuesId });
                    table.ForeignKey(
                        name: "FK_CustomFieldOptionItemCustomFieldValue_CustomFieldOption_Val~",
                        column: x => x.ValuesId,
                        principalTable: "CustomFieldOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomFieldOptionItemCustomFieldValue_ItemCustomFieldValue_~",
                        column: x => x.SelectedOnId,
                        principalTable: "ItemCustomFieldValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomFieldOptionItemCustomFieldValue_ValuesId",
                table: "CustomFieldOptionItemCustomFieldValue",
                column: "ValuesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomFieldOptionItemCustomFieldValue");

            migrationBuilder.AddColumn<int>(
                name: "ItemCustomFieldValueId",
                table: "CustomFieldOption",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomFieldOption_ItemCustomFieldValueId",
                table: "CustomFieldOption",
                column: "ItemCustomFieldValueId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomFieldOption_ItemCustomFieldValue_ItemCustomFieldValue~",
                table: "CustomFieldOption",
                column: "ItemCustomFieldValueId",
                principalTable: "ItemCustomFieldValue",
                principalColumn: "Id");
        }
    }
}
