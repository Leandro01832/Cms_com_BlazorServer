﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class ContentPagina : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Content_Filtro_FiltroId",
                table: "Content");

            migrationBuilder.DropIndex(
                name: "IX_Content_FiltroId",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "FiltroId",
                table: "Content");

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

            migrationBuilder.CreateTable(
                name: "ContentPagina",
                columns: table => new
                {
                    PaginaId = table.Column<long>(type: "bigint", nullable: false),
                    ContentId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPagina", x => new { x.ContentId, x.PaginaId });
                    table.ForeignKey(
                        name: "FK_ContentPagina_Content_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentPagina_Pagina_PaginaId",
                        column: x => x.PaginaId,
                        principalTable: "Pagina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentFiltro_FiltroId",
                table: "ContentFiltro",
                column: "FiltroId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPagina_PaginaId",
                table: "ContentPagina",
                column: "PaginaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentFiltro");

            migrationBuilder.DropTable(
                name: "ContentPagina");

            migrationBuilder.AddColumn<long>(
                name: "FiltroId",
                table: "Content",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Content_FiltroId",
                table: "Content",
                column: "FiltroId");

            migrationBuilder.AddForeignKey(
                name: "FK_Content_Filtro_FiltroId",
                table: "Content",
                column: "FiltroId",
                principalTable: "Filtro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}