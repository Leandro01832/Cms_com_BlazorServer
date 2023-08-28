using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class classificacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Desconto",
                table: "Compartilhante",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Classificacao",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    preferencia1 = table.Column<int>(type: "int", nullable: false),
                    preferencia2 = table.Column<int>(type: "int", nullable: false),
                    preferencia3 = table.Column<int>(type: "int", nullable: false),
                    preferencia4 = table.Column<int>(type: "int", nullable: false),
                    preferencia5 = table.Column<int>(type: "int", nullable: false),
                    preferencia6 = table.Column<int>(type: "int", nullable: false),
                    preferencia7 = table.Column<int>(type: "int", nullable: false),
                    preferencia8 = table.Column<int>(type: "int", nullable: false),
                    preferencia9 = table.Column<int>(type: "int", nullable: false),
                    preferencia10 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classificacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classificacao_Pagina_Id",
                        column: x => x.Id,
                        principalTable: "Pagina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Classificacao");

            migrationBuilder.DropColumn(
                name: "Desconto",
                table: "Compartilhante");
        }
    }
}
