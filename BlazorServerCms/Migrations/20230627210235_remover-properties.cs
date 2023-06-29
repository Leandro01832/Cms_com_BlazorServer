using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class removerproperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Layout",
                table: "Pagina");

            migrationBuilder.DropColumn(
                name: "Tempo",
                table: "Pagina");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Layout",
                table: "Pagina",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Tempo",
                table: "Pagina",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
