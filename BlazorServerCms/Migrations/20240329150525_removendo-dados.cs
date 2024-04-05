using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class removendodados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Content_highlighter_highlighterId",
                table: "Content");

            migrationBuilder.DropTable(
                name: "MarcadorPagina");

            migrationBuilder.DropTable(
                name: "highlighter");

            migrationBuilder.RenameColumn(
                name: "highlighterId",
                table: "Content",
                newName: "FiltroId");

            migrationBuilder.RenameIndex(
                name: "IX_Content_highlighterId",
                table: "Content",
                newName: "IX_Content_FiltroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Content_Filtro_FiltroId",
                table: "Content",
                column: "FiltroId",
                principalTable: "Filtro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Content_Filtro_FiltroId",
                table: "Content");

            migrationBuilder.RenameColumn(
                name: "FiltroId",
                table: "Content",
                newName: "highlighterId");

            migrationBuilder.RenameIndex(
                name: "IX_Content_FiltroId",
                table: "Content",
                newName: "IX_Content_highlighterId");

            migrationBuilder.CreateTable(
                name: "highlighter",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    capitulo = table.Column<int>(type: "int", nullable: false),
                    pasta = table.Column<int>(type: "int", nullable: false),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_highlighter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarcadorPagina",
                columns: table => new
                {
                    highlighterId = table.Column<long>(type: "bigint", nullable: false),
                    PaginaId = table.Column<long>(type: "bigint", nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Content_highlighter_highlighterId",
                table: "Content",
                column: "highlighterId",
                principalTable: "highlighter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
