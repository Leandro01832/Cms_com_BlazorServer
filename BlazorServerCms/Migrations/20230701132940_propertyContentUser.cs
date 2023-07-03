using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class propertyContentUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentUser",
                table: "Pagina",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentUser",
                table: "Pagina");
        }
    }
}
