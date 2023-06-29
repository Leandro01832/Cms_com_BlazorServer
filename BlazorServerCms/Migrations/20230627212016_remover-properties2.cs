using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class removerproperties2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArquivoMusic",
                table: "Pagina");

            migrationBuilder.DropColumn(
                name: "Music",
                table: "Pagina");

            migrationBuilder.DropColumn(
                name: "Sobreescrita",
                table: "Pagina");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArquivoMusic",
                table: "Pagina",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Music",
                table: "Pagina",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Sobreescrita",
                table: "Pagina",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
