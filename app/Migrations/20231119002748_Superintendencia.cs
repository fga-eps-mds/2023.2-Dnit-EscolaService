using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class Superintendencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DistanciaSuperintendencia",
                table: "Escolas",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SuperintendenciaId",
                table: "Escolas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Superintendencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Endereco = table.Column<string>(type: "text", nullable: false),
                    Cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Latitude = table.Column<string>(type: "text", nullable: false),
                    Longitude = table.Column<string>(type: "text", nullable: false),
                    Uf = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Superintendencias", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Escolas_SuperintendenciaId",
                table: "Escolas",
                column: "SuperintendenciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Escolas_Superintendencias_SuperintendenciaId",
                table: "Escolas",
                column: "SuperintendenciaId",
                principalTable: "Superintendencias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Escolas_Superintendencias_SuperintendenciaId",
                table: "Escolas");

            migrationBuilder.DropTable(
                name: "Superintendencias");

            migrationBuilder.DropIndex(
                name: "IX_Escolas_SuperintendenciaId",
                table: "Escolas");

            migrationBuilder.DropColumn(
                name: "DistanciaSuperintendencia",
                table: "Escolas");

            migrationBuilder.DropColumn(
                name: "SuperintendenciaId",
                table: "Escolas");
        }
    }
}
