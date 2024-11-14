using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class conteudousuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user",
                table: "Content");

            migrationBuilder.AddColumn<string>(
                name: "UserModelId",
                table: "Content",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Content_UserModelId",
                table: "Content",
                column: "UserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Content_AspNetUsers_UserModelId",
                table: "Content",
                column: "UserModelId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Content_AspNetUsers_UserModelId",
                table: "Content");

            migrationBuilder.DropIndex(
                name: "IX_Content_UserModelId",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "Content");

            migrationBuilder.AddColumn<string>(
                name: "user",
                table: "Content",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
