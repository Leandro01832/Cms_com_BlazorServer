using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class livros : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LivroCompartilhado");

            migrationBuilder.AddColumn<long>(
                name: "InstanteId",
                table: "Livro",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "capitulo",
                table: "Livro",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "pasta",
                table: "Livro",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "user",
                table: "Livro",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "versiculo",
                table: "Livro",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Livro_Instante_InstanteId",
                table: "Livro");

            migrationBuilder.DropIndex(
                name: "IX_Livro_InstanteId",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "InstanteId",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "capitulo",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "pasta",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "user",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "versiculo",
                table: "Livro");

            migrationBuilder.CreateTable(
                name: "LivroCompartilhado",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstanteId = table.Column<long>(type: "bigint", nullable: false),
                    LivroId = table.Column<long>(type: "bigint", nullable: false),
                    capitulo = table.Column<int>(type: "int", nullable: false),
                    pasta = table.Column<int>(type: "int", nullable: false),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    versiculo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivroCompartilhado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LivroCompartilhado_Instante_InstanteId",
                        column: x => x.InstanteId,
                        principalTable: "Instante",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LivroCompartilhado_Livro_LivroId",
                        column: x => x.LivroId,
                        principalTable: "Livro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LivroCompartilhado_InstanteId",
                table: "LivroCompartilhado",
                column: "InstanteId");

            migrationBuilder.CreateIndex(
                name: "IX_LivroCompartilhado_LivroId",
                table: "LivroCompartilhado",
                column: "LivroId");
        }
    }
}
