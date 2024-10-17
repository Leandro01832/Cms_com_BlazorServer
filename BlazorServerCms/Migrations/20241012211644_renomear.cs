using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class renomear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Content_Filtro_FiltroId",
                table: "Content");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentPagina_Content_ContentId",
                table: "ContentPagina");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentPagina_Filtro_FiltroId",
                table: "ContentPagina");

            migrationBuilder.DropIndex(
                name: "IX_Content_FiltroId",
                table: "Content");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentPagina",
                table: "ContentPagina");

            migrationBuilder.DropColumn(
                name: "FiltroId",
                table: "Content");

            migrationBuilder.RenameTable(
                name: "ContentPagina",
                newName: "ContentFiltro");

            migrationBuilder.RenameIndex(
                name: "IX_ContentPagina_FiltroId",
                table: "ContentFiltro",
                newName: "IX_ContentFiltro_FiltroId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentFiltro",
                table: "ContentFiltro",
                columns: new[] { "ContentId", "FiltroId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ContentFiltro_Content_ContentId",
                table: "ContentFiltro",
                column: "ContentId",
                principalTable: "Content",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentFiltro_Filtro_FiltroId",
                table: "ContentFiltro",
                column: "FiltroId",
                principalTable: "Filtro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentFiltro_Content_ContentId",
                table: "ContentFiltro");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentFiltro_Filtro_FiltroId",
                table: "ContentFiltro");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentFiltro",
                table: "ContentFiltro");

            migrationBuilder.RenameTable(
                name: "ContentFiltro",
                newName: "ContentPagina");

            migrationBuilder.RenameIndex(
                name: "IX_ContentFiltro_FiltroId",
                table: "ContentPagina",
                newName: "IX_ContentPagina_FiltroId");

            migrationBuilder.AddColumn<long>(
                name: "FiltroId",
                table: "Content",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentPagina",
                table: "ContentPagina",
                columns: new[] { "ContentId", "FiltroId" });

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
                name: "FK_ContentPagina_Content_ContentId",
                table: "ContentPagina",
                column: "ContentId",
                principalTable: "Content",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentPagina_Filtro_FiltroId",
                table: "ContentPagina",
                column: "FiltroId",
                principalTable: "Filtro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
