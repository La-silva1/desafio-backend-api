using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DesafioBackend.Migrations
{
    /// <inheritdoc />
    public partial class CreateEntregadoresTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entregadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Cnpj = table.Column<string>(type: "text", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NumeroCNH = table.Column<string>(type: "text", nullable: false),
                    TipoCNH = table.Column<string>(type: "text", nullable: false),
                    FotoCNH = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entregadores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entregadores_Cnpj",
                table: "Entregadores",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entregadores_NumeroCNH",
                table: "Entregadores",
                column: "NumeroCNH",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entregadores");
        }
    }
}
