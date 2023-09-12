using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class removendoproperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescontoDoCompartilhante",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "ComissaoParaUser",
                table: "Compartilhante");

            migrationBuilder.DropColumn(
                name: "Desconto",
                table: "Compartilhante");

            migrationBuilder.AddColumn<string>(
                name: "Admin",
                table: "Compartilhante",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Admin",
                table: "Compartilhante");

            migrationBuilder.AddColumn<int>(
                name: "DescontoDoCompartilhante",
                table: "Livro",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ComissaoParaUser",
                table: "Compartilhante",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Desconto",
                table: "Compartilhante",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
