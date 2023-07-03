using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class propertyDesconto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DescontoDoCompartilhante",
                table: "Livro",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescontoDoCompartilhante",
                table: "Livro");
        }
    }
}
