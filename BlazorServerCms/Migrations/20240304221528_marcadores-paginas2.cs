using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class marcadorespaginas2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarcadorPagina",
                columns: table => new
                {
                    PaginaId = table.Column<long>(type: "bigint", nullable: false),
                    highlighterId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarcadorPagina", x => new { x.highlighterId, x.PaginaId });
                    table.ForeignKey(
                        name: "FK_MarcadorPagina_highlighter_highlighterId",
                        column: x => x.highlighterId,
                        principalTable: "highlighter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarcadorPagina_Pagina_PaginaId",
                        column: x => x.PaginaId,
                        principalTable: "Pagina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarcadorPagina_PaginaId",
                table: "MarcadorPagina",
                column: "PaginaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarcadorPagina");
        }
    }
}
