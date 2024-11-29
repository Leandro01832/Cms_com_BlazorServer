using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class RemovePageLiked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserModelPageLiked_PageLiked_PageLikedId",
                table: "UserModelPageLiked");

            migrationBuilder.DropTable(
                name: "PageLiked");

            migrationBuilder.RenameColumn(
                name: "PageLikedId",
                table: "UserModelPageLiked",
                newName: "ContentId");

            migrationBuilder.RenameIndex(
                name: "IX_UserModelPageLiked_PageLikedId",
                table: "UserModelPageLiked",
                newName: "IX_UserModelPageLiked_ContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserModelPageLiked_Content_ContentId",
                table: "UserModelPageLiked",
                column: "ContentId",
                principalTable: "Content",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserModelPageLiked_Content_ContentId",
                table: "UserModelPageLiked");

            migrationBuilder.RenameColumn(
                name: "ContentId",
                table: "UserModelPageLiked",
                newName: "PageLikedId");

            migrationBuilder.RenameIndex(
                name: "IX_UserModelPageLiked_ContentId",
                table: "UserModelPageLiked",
                newName: "IX_UserModelPageLiked_PageLikedId");

            migrationBuilder.CreateTable(
                name: "PageLiked",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentId = table.Column<long>(type: "bigint", nullable: true),
                    PaginaId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageLiked", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageLiked_Content_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Content",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PageLiked_Content_PaginaId",
                        column: x => x.PaginaId,
                        principalTable: "Content",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageLiked_ContentId",
                table: "PageLiked",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_PageLiked_PaginaId",
                table: "PageLiked",
                column: "PaginaId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserModelPageLiked_PageLiked_PageLikedId",
                table: "UserModelPageLiked",
                column: "PageLikedId",
                principalTable: "PageLiked",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
