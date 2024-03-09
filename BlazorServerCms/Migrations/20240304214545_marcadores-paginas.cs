using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class marcadorespaginas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "verso1",
                table: "highlighter");

            migrationBuilder.DropColumn(
                name: "verso10",
                table: "highlighter");

            migrationBuilder.DropColumn(
                name: "verso2",
                table: "highlighter");

            migrationBuilder.DropColumn(
                name: "verso3",
                table: "highlighter");

            migrationBuilder.DropColumn(
                name: "verso4",
                table: "highlighter");

            migrationBuilder.DropColumn(
                name: "verso5",
                table: "highlighter");

            migrationBuilder.DropColumn(
                name: "verso6",
                table: "highlighter");

            migrationBuilder.DropColumn(
                name: "verso7",
                table: "highlighter");

            migrationBuilder.DropColumn(
                name: "verso8",
                table: "highlighter");

            migrationBuilder.DropColumn(
                name: "verso9",
                table: "highlighter");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "verso1",
                table: "highlighter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "verso10",
                table: "highlighter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "verso2",
                table: "highlighter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "verso3",
                table: "highlighter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "verso4",
                table: "highlighter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "verso5",
                table: "highlighter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "verso6",
                table: "highlighter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "verso7",
                table: "highlighter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "verso8",
                table: "highlighter",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "verso9",
                table: "highlighter",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
