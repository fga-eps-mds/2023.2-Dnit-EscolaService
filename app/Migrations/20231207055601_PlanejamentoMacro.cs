using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class PlanejamentoMacro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanejamentoMacro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Responsavel = table.Column<string>(type: "text", nullable: false),
                    MesInicio = table.Column<int>(type: "integer", nullable: false),
                    MesFim = table.Column<int>(type: "integer", nullable: false),
                    AnoInicio = table.Column<string>(type: "text", nullable: false),
                    AnoFim = table.Column<string>(type: "text", nullable: false),
                    QuantidadeAcoes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanejamentoMacro", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanejamentoMacroEscola",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Mes = table.Column<int>(type: "integer", nullable: false),
                    Ano = table.Column<string>(type: "text", nullable: false),
                    PlanejamentoMacroId = table.Column<Guid>(type: "uuid", nullable: false),
                    EscolaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanejamentoMacroEscola", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanejamentoMacroEscola_Escolas_EscolaId",
                        column: x => x.EscolaId,
                        principalTable: "Escolas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanejamentoMacroEscola_PlanejamentoMacro_PlanejamentoMacro~",
                        column: x => x.PlanejamentoMacroId,
                        principalTable: "PlanejamentoMacro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanejamentoMacroEscola_EscolaId",
                table: "PlanejamentoMacroEscola",
                column: "EscolaId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanejamentoMacroEscola_PlanejamentoMacroId",
                table: "PlanejamentoMacroEscola",
                column: "PlanejamentoMacroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanejamentoMacroEscola");

            migrationBuilder.DropTable(
                name: "PlanejamentoMacro");
        }
    }
}
