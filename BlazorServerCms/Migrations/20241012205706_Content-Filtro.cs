using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class ContentFiltro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentPagina_Pagina_PaginaId",
                table: "ContentPagina");

            migrationBuilder.DropTable(
                name: "ContentFiltro");

            migrationBuilder.RenameColumn(
                name: "PaginaId",
                table: "ContentPagina",
                newName: "FiltroId");

            migrationBuilder.RenameIndex(
                name: "IX_ContentPagina_PaginaId",
                table: "ContentPagina",
                newName: "IX_ContentPagina_FiltroId");

            migrationBuilder.AddColumn<long>(
                name: "FiltroId",
                table: "Content",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Content_FiltroId",
                table: "Content",
                column: "FiltroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Content_Filtro_FiltroId",
                table: "Content",
                column: "FiltroId",
                principalTable: "Filtro",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentPagina_Filtro_FiltroId",
                table: "ContentPagina",
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

            migrationBuilder.DropForeignKey(
                name: "FK_ContentPagina_Filtro_FiltroId",
                table: "ContentPagina");

            migrationBuilder.DropIndex(
                name: "IX_Content_FiltroId",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "FiltroId",
                table: "Content");

            migrationBuilder.RenameColumn(
                name: "FiltroId",
                table: "ContentPagina",
                newName: "PaginaId");

            migrationBuilder.RenameIndex(
                name: "IX_ContentPagina_FiltroId",
                table: "ContentPagina",
                newName: "IX_ContentPagina_PaginaId");

            migrationBuilder.CreateTable(
                name: "ContentFiltro",
                columns: table => new
                {
                    ContentId = table.Column<long>(type: "bigint", nullable: false),
                    FiltroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentFiltro", x => new { x.ContentId, x.FiltroId });
                    table.ForeignKey(
                        name: "FK_ContentFiltro_Content_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentFiltro_Filtro_FiltroId",
                        column: x => x.FiltroId,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentFiltro_FiltroId",
                table: "ContentFiltro",
                column: "FiltroId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentPagina_Pagina_PaginaId",
                table: "ContentPagina",
                column: "PaginaId",
                principalTable: "Pagina",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
