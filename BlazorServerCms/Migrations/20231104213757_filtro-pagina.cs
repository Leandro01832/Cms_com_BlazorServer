using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class filtropagina : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "FiltroPagina");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "FiltroPagina",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
