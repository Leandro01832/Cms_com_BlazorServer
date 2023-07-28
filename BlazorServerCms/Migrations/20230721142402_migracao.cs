using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class migracao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Compartilhamento",
                table: "Compartilhamento");

            migrationBuilder.RenameTable(
                name: "Compartilhamento",
                newName: "Compartilhante");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Compartilhante",
                table: "Compartilhante",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Compartilhante",
                table: "Compartilhante");

            migrationBuilder.RenameTable(
                name: "Compartilhante",
                newName: "Compartilhamento");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Compartilhamento",
                table: "Compartilhamento",
                column: "Id");
        }
    }
}
