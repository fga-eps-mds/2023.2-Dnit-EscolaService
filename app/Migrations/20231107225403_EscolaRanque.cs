using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class EscolaRanque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EscolaRanques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EscolaId = table.Column<Guid>(type: "uuid", nullable: false),
                    RanqueId = table.Column<int>(type: "integer", nullable: false),
                    Pontuacao = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscolaRanques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EscolaRanques_Escolas_EscolaId",
                        column: x => x.EscolaId,
                        principalTable: "Escolas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EscolaRanques_Ranques_RanqueId",
                        column: x => x.RanqueId,
                        principalTable: "Ranques",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EscolaRanques_EscolaId",
                table: "EscolaRanques",
                column: "EscolaId");

            migrationBuilder.CreateIndex(
                name: "IX_EscolaRanques_RanqueId",
                table: "EscolaRanques",
                column: "RanqueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EscolaRanques");
        }
    }
}
