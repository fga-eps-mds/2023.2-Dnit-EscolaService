using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class FatorPriorizacaoSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustosLogisticos",
                columns: table => new
                {
                    Custo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Valor = table.Column<int>(type: "integer", nullable: false),
                    RaioMin = table.Column<int>(type: "integer", nullable: false),
                    RaioMax = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustosLogisticos", x => x.Custo);
                });

            migrationBuilder.CreateTable(
                name: "FatorPriorizacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Peso = table.Column<int>(type: "integer", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Primario = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FatorPriorizacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FatorCondicoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Propriedade = table.Column<int>(type: "integer", maxLength: 30, nullable: false),
                    Operador = table.Column<int>(type: "integer", nullable: false),
                    FatorPriorizacaoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FatorCondicoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FatorCondicoes_FatorPriorizacoes_FatorPriorizacaoId",
                        column: x => x.FatorPriorizacaoId,
                        principalTable: "FatorPriorizacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FatorEscolas",
                columns: table => new
                {
                    FatorPriorizacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    EscolaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Valor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FatorEscolas", x => new { x.FatorPriorizacaoId, x.EscolaId });
                    table.ForeignKey(
                        name: "FK_FatorEscolas_Escolas_EscolaId",
                        column: x => x.EscolaId,
                        principalTable: "Escolas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FatorEscolas_FatorPriorizacoes_FatorPriorizacaoId",
                        column: x => x.FatorPriorizacaoId,
                        principalTable: "FatorPriorizacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FatorRanques",
                columns: table => new
                {
                    FatorPriorizacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    RanqueId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FatorRanques", x => new { x.FatorPriorizacaoId, x.RanqueId });
                    table.ForeignKey(
                        name: "FK_FatorRanques_FatorPriorizacoes_FatorPriorizacaoId",
                        column: x => x.FatorPriorizacaoId,
                        principalTable: "FatorPriorizacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FatorRanques_Ranques_RanqueId",
                        column: x => x.RanqueId,
                        principalTable: "Ranques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CondicaoValores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FatorCondicaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Valor = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondicaoValores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CondicaoValores_FatorCondicoes_FatorCondicaoId",
                        column: x => x.FatorCondicaoId,
                        principalTable: "FatorCondicoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CondicaoValores_FatorCondicaoId",
                table: "CondicaoValores",
                column: "FatorCondicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_FatorCondicoes_FatorPriorizacaoId",
                table: "FatorCondicoes",
                column: "FatorPriorizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_FatorEscolas_EscolaId",
                table: "FatorEscolas",
                column: "EscolaId");

            migrationBuilder.CreateIndex(
                name: "IX_FatorRanques_RanqueId",
                table: "FatorRanques",
                column: "RanqueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CondicaoValores");

            migrationBuilder.DropTable(
                name: "CustosLogisticos");

            migrationBuilder.DropTable(
                name: "FatorEscolas");

            migrationBuilder.DropTable(
                name: "FatorRanques");

            migrationBuilder.DropTable(
                name: "FatorCondicoes");

            migrationBuilder.DropTable(
                name: "FatorPriorizacoes");
        }
    }
}
