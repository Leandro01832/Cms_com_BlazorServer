using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class Livro2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserResponse");

            migrationBuilder.DropTable(
                name: "Pergunta");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Pergunta",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FiltroId = table.Column<long>(type: "bigint", nullable: false),
                    Questao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseChatGpt = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pergunta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pergunta_Filtro_FiltroId",
                        column: x => x.FiltroId,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserResponse",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    LivroId = table.Column<long>(type: "bigint", nullable: true),
                    capitulo = table.Column<int>(type: "int", nullable: false),
                    exempoloR1 = table.Column<bool>(type: "bit", nullable: false),
                    exempoloR10 = table.Column<bool>(type: "bit", nullable: false),
                    exempoloR2 = table.Column<bool>(type: "bit", nullable: false),
                    exempoloR3 = table.Column<bool>(type: "bit", nullable: false),
                    exempoloR4 = table.Column<bool>(type: "bit", nullable: false),
                    exempoloR5 = table.Column<bool>(type: "bit", nullable: false),
                    exempoloR6 = table.Column<bool>(type: "bit", nullable: false),
                    exempoloR7 = table.Column<bool>(type: "bit", nullable: false),
                    exempoloR8 = table.Column<bool>(type: "bit", nullable: false),
                    exempoloR9 = table.Column<bool>(type: "bit", nullable: false),
                    pasta = table.Column<int>(type: "int", nullable: false),
                    resposta1 = table.Column<int>(type: "int", nullable: false),
                    resposta10 = table.Column<int>(type: "int", nullable: false),
                    resposta2 = table.Column<int>(type: "int", nullable: false),
                    resposta3 = table.Column<int>(type: "int", nullable: false),
                    resposta4 = table.Column<int>(type: "int", nullable: false),
                    resposta5 = table.Column<int>(type: "int", nullable: false),
                    resposta6 = table.Column<int>(type: "int", nullable: false),
                    resposta7 = table.Column<int>(type: "int", nullable: false),
                    resposta8 = table.Column<int>(type: "int", nullable: false),
                    resposta9 = table.Column<int>(type: "int", nullable: false),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserResponse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserResponse_Livro_LivroId",
                        column: x => x.LivroId,
                        principalTable: "Livro",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserResponse_Pergunta_Id",
                        column: x => x.Id,
                        principalTable: "Pergunta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pergunta_FiltroId",
                table: "Pergunta",
                column: "FiltroId");

            migrationBuilder.CreateIndex(
                name: "IX_UserResponse_LivroId",
                table: "UserResponse",
                column: "LivroId");
        }
    }
}
