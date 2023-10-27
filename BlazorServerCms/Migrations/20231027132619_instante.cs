using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class instante : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InstanteAdminId",
                table: "Livro",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InstanteUserId",
                table: "Livro",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pasta",
                table: "Livro",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InstanteAdmin",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstanteAdmin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstanteUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstanteUser", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Livro_InstanteAdminId",
                table: "Livro",
                column: "InstanteAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Livro_InstanteUserId",
                table: "Livro",
                column: "InstanteUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Livro_InstanteAdmin_InstanteAdminId",
                table: "Livro",
                column: "InstanteAdminId",
                principalTable: "InstanteAdmin",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Livro_InstanteUser_InstanteUserId",
                table: "Livro",
                column: "InstanteUserId",
                principalTable: "InstanteUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Livro_InstanteAdmin_InstanteAdminId",
                table: "Livro");

            migrationBuilder.DropForeignKey(
                name: "FK_Livro_InstanteUser_InstanteUserId",
                table: "Livro");

            migrationBuilder.DropTable(
                name: "InstanteAdmin");

            migrationBuilder.DropTable(
                name: "InstanteUser");

            migrationBuilder.DropIndex(
                name: "IX_Livro_InstanteAdminId",
                table: "Livro");

            migrationBuilder.DropIndex(
                name: "IX_Livro_InstanteUserId",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "InstanteAdminId",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "InstanteUserId",
                table: "Livro");

            migrationBuilder.DropColumn(
                name: "Pasta",
                table: "Livro");
        }
    }
}
