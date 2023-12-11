using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class Refatora_Superintendencias_para_Polos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Escolas_Superintendencias_SuperintendenciaId",
                table: "Escolas");

            migrationBuilder.DropTable(
                name: "Superintendencias");

            migrationBuilder.RenameColumn(
                name: "SuperintendenciaId",
                table: "Escolas",
                newName: "PoloId");

            migrationBuilder.RenameColumn(
                name: "DistanciaSuperintendencia",
                table: "Escolas",
                newName: "DistanciaPolo");

            migrationBuilder.RenameIndex(
                name: "IX_Escolas_SuperintendenciaId",
                table: "Escolas",
                newName: "IX_Escolas_PoloId");

            migrationBuilder.CreateTable(
                name: "Polos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Endereco = table.Column<string>(type: "text", nullable: false),
                    Cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Latitude = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    MunicipioId = table.Column<int>(type: "integer", nullable: false),
                    Longitude = table.Column<string>(type: "text", nullable: false),
                    Uf = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Polos_Municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "Municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Polos_MunicipioId",
                table: "Polos",
                column: "MunicipioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Escolas_Polos_PoloId",
                table: "Escolas",
                column: "PoloId",
                principalTable: "Polos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Escolas_Polos_PoloId",
                table: "Escolas");

            migrationBuilder.DropTable(
                name: "Polos");

            migrationBuilder.RenameColumn(
                name: "PoloId",
                table: "Escolas",
                newName: "SuperintendenciaId");

            migrationBuilder.RenameColumn(
                name: "DistanciaPolo",
                table: "Escolas",
                newName: "DistanciaSuperintendencia");

            migrationBuilder.RenameIndex(
                name: "IX_Escolas_PoloId",
                table: "Escolas",
                newName: "IX_Escolas_SuperintendenciaId");

            migrationBuilder.CreateTable(
                name: "Superintendencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Endereco = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<string>(type: "text", nullable: false),
                    Longitude = table.Column<string>(type: "text", nullable: false),
                    Uf = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Superintendencias", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Escolas_Superintendencias_SuperintendenciaId",
                table: "Escolas",
                column: "SuperintendenciaId",
                principalTable: "Superintendencias",
                principalColumn: "Id");
        }
    }
}
