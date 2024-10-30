using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class filtroopcional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Content_Filtro_FiltroId",
                table: "Content");

            migrationBuilder.AlterColumn<long>(
                name: "FiltroId",
                table: "Content",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Content_Filtro_FiltroId",
                table: "Content",
                column: "FiltroId",
                principalTable: "Filtro",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Content_Filtro_FiltroId",
                table: "Content");

            migrationBuilder.AlterColumn<long>(
                name: "FiltroId",
                table: "Content",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

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
