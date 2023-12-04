using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaSolicitacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Solicitacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EscolaUf = table.Column<int>(type: "integer", nullable: false),
                    EscolaMunicipioId = table.Column<int>(type: "integer", nullable: false),
                    EscolaCodigoInep = table.Column<int>(type: "integer", nullable: false),
                    EscolaNome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TotalAlunos = table.Column<int>(type: "integer", nullable: false),
                    EscolaId = table.Column<Guid>(type: "uuid", nullable: true),
                    NomeSolicitante = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Vinculo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DataRealizadaUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solicitacoes_Escolas_EscolaId",
                        column: x => x.EscolaId,
                        principalTable: "Escolas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Solicitacoes_Municipios_EscolaMunicipioId",
                        column: x => x.EscolaMunicipioId,
                        principalTable: "Municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solicitacoes_EscolaId",
                table: "Solicitacoes",
                column: "EscolaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solicitacoes_EscolaMunicipioId",
                table: "Solicitacoes",
                column: "EscolaMunicipioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Solicitacoes");
        }
    }
}
