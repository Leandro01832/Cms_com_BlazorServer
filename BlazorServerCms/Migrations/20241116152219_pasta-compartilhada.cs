using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class pastacompartilhada : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FiltroId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FiltroId",
                table: "AspNetUsers",
                column: "FiltroId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Filtro_FiltroId",
                table: "AspNetUsers",
                column: "FiltroId",
                principalTable: "Filtro",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Filtro_FiltroId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FiltroId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FiltroId",
                table: "AspNetUsers");
        }
    }
}
