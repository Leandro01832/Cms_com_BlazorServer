using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class Livro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Livro_Instante_InstanteId",
                table: "Livro");

            migrationBuilder.DropTable(
                name: "Instante");

            migrationBuilder.DropIndex(
                name: "IX_Livro_InstanteId",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "InstanteId",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "UrlNoBook",
                table: "Livro");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InstanteId",
                table: "Livro",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlNoBook",
                table: "Livro",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Instante",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instante", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Livro_InstanteId",
                table: "Livro",
                column: "InstanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Livro_Instante_InstanteId",
                table: "Livro",
                column: "InstanteId",
                principalTable: "Instante",
                principalColumn: "Id");
        }
    }
}
