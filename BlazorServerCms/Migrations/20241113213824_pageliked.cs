using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class pageliked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "camadaDez",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "camadaNove",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "camadaOito",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "camadaSeis",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "camadaSete",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "capitulo",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "grupo",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "indice",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "subgrupo",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "substory",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "subsubgrupo",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "user",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "verso",
                table: "PageLiked");

            migrationBuilder.AddColumn<long>(
                name: "ContentId",
                table: "PageLiked",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PaginaId",
                table: "PageLiked",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserModelPageLiked",
                columns: table => new
                {
                    UserModelId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PageLikedId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModelPageLiked", x => new { x.UserModelId, x.PageLikedId });
                    table.ForeignKey(
                        name: "FK_UserModelPageLiked_AspNetUsers_UserModelId",
                        column: x => x.UserModelId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserModelPageLiked_PageLiked_PageLikedId",
                        column: x => x.PageLikedId,
                        principalTable: "PageLiked",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageLiked_ContentId",
                table: "PageLiked",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_PageLiked_PaginaId",
                table: "PageLiked",
                column: "PaginaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserModelPageLiked_PageLikedId",
                table: "UserModelPageLiked",
                column: "PageLikedId");

            migrationBuilder.AddForeignKey(
                name: "FK_PageLiked_Content_ContentId",
                table: "PageLiked",
                column: "ContentId",
                principalTable: "Content",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PageLiked_Pagina_PaginaId",
                table: "PageLiked",
                column: "PaginaId",
                principalTable: "Pagina",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageLiked_Content_ContentId",
                table: "PageLiked");

            migrationBuilder.DropForeignKey(
                name: "FK_PageLiked_Pagina_PaginaId",
                table: "PageLiked");

            migrationBuilder.DropTable(
                name: "UserModelPageLiked");

            migrationBuilder.DropIndex(
                name: "IX_PageLiked_ContentId",
                table: "PageLiked");

            migrationBuilder.DropIndex(
                name: "IX_PageLiked_PaginaId",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "ContentId",
                table: "PageLiked");

            migrationBuilder.DropColumn(
                name: "PaginaId",
                table: "PageLiked");

            migrationBuilder.AddColumn<int>(
                name: "camadaDez",
                table: "PageLiked",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "camadaNove",
                table: "PageLiked",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "camadaOito",
                table: "PageLiked",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "camadaSeis",
                table: "PageLiked",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "camadaSete",
                table: "PageLiked",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "capitulo",
                table: "PageLiked",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "grupo",
                table: "PageLiked",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "indice",
                table: "PageLiked",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "subgrupo",
                table: "PageLiked",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "substory",
                table: "PageLiked",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "subsubgrupo",
                table: "PageLiked",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user",
                table: "PageLiked",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "verso",
                table: "PageLiked",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
