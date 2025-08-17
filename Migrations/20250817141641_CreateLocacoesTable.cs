using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DesafioBackend.Migrations
{
    /// <inheritdoc />
    public partial class CreateLocacoesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locacoes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EntregadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    MotoId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataTermino = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataPrevisaoTermino = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Plano = table.Column<string>(type: "text", nullable: false),
                    CustoTotal = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locacoes_Entregadores_EntregadorId",
                        column: x => x.EntregadorId,
                        principalTable: "Entregadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Locacoes_Motos_MotoId",
                        column: x => x.MotoId,
                        principalTable: "Motos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_EntregadorId",
                table: "Locacoes",
                column: "EntregadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Locacoes_MotoId",
                table: "Locacoes",
                column: "MotoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locacoes");
        }
    }
}
