using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class Content : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageLiked_Pagina_PaginaId",
                table: "PageLiked");

            migrationBuilder.DropForeignKey(
                name: "FK_Produto_Pagina_Id",
                table: "Produto");

            migrationBuilder.DropTable(
                name: "FiltroPagina");

            migrationBuilder.DropTable(
                name: "Pagina");

            migrationBuilder.AlterColumn<string>(
                name: "UserModelId",
                table: "Content",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Html",
                table: "Content",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<long>(
                name: "Comentario",
                table: "Content",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Content",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rotas",
                table: "Content",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StoryId",
                table: "Content",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "Content",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Versiculo",
                table: "Content",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FiltroContent",
                columns: table => new
                {
                    ContentId = table.Column<long>(type: "bigint", nullable: false),
                    FiltroId = table.Column<long>(type: "bigint", nullable: false),
                    QuantidadePorType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiltroContent", x => new { x.FiltroId, x.ContentId });
                    table.ForeignKey(
                        name: "FK_FiltroContent_Content_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FiltroContent_Filtro_FiltroId",
                        column: x => x.FiltroId,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MudancaEstado",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pontos = table.Column<int>(type: "int", nullable: false),
                    Curtidas = table.Column<long>(type: "bigint", nullable: false),
                    Compartilhamentos = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MudancaEstado", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Content_StoryId",
                table: "Content",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FiltroContent_ContentId",
                table: "FiltroContent",
                column: "ContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Content_Story_StoryId",
                table: "Content",
                column: "StoryId",
                principalTable: "Story",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PageLiked_Content_PaginaId",
                table: "PageLiked",
                column: "PaginaId",
                principalTable: "Content",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_Content_Id",
                table: "Produto",
                column: "Id",
                principalTable: "Content",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Content_Story_StoryId",
                table: "Content");

            migrationBuilder.DropForeignKey(
                name: "FK_PageLiked_Content_PaginaId",
                table: "PageLiked");

            migrationBuilder.DropForeignKey(
                name: "FK_Produto_Content_Id",
                table: "Produto");

            migrationBuilder.DropTable(
                name: "FiltroContent");

            migrationBuilder.DropTable(
                name: "MudancaEstado");

            migrationBuilder.DropIndex(
                name: "IX_Content_StoryId",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "Comentario",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "Rotas",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "Versiculo",
                table: "Content");

            migrationBuilder.AlterColumn<string>(
                name: "UserModelId",
                table: "Content",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Html",
                table: "Content",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Pagina",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoryId = table.Column<long>(type: "bigint", nullable: false),
                    Comentario = table.Column<long>(type: "bigint", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rotas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Versiculo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagina_Story_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Story",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FiltroPagina",
                columns: table => new
                {
                    FiltroId = table.Column<long>(type: "bigint", nullable: false),
                    PaginaId = table.Column<long>(type: "bigint", nullable: false),
                    QuantidadePorType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiltroPagina", x => new { x.FiltroId, x.PaginaId });
                    table.ForeignKey(
                        name: "FK_FiltroPagina_Filtro_FiltroId",
                        column: x => x.FiltroId,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FiltroPagina_Pagina_PaginaId",
                        column: x => x.PaginaId,
                        principalTable: "Pagina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FiltroPagina_PaginaId",
                table: "FiltroPagina",
                column: "PaginaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagina_StoryId",
                table: "Pagina",
                column: "StoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageLiked_Pagina_PaginaId",
                table: "PageLiked",
                column: "PaginaId",
                principalTable: "Pagina",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Produto_Pagina_Id",
                table: "Produto",
                column: "Id",
                principalTable: "Pagina",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
