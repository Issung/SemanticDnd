using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pgvector;

#nullable disable

namespace VectorSimple.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "TestModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Embedding = table.Column<Vector>(type: "vector(768)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestModels_Embedding",
                table: "TestModels",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "ivfflat")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_l2_ops" })
                .Annotation("Npgsql:StorageParameter:lists", 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestModels");
        }
    }
}
