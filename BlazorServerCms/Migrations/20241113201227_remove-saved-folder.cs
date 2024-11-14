using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class removesavedfolder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "savedFolder");

            migrationBuilder.CreateTable(
                name: "UserModelFiltro",
                columns: table => new
                {
                    UserModelId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FiltroId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModelFiltro", x => new { x.UserModelId, x.FiltroId });
                    table.ForeignKey(
                        name: "FK_UserModelFiltro_AspNetUsers_UserModelId",
                        column: x => x.UserModelId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserModelFiltro_Filtro_FiltroId",
                        column: x => x.FiltroId,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserModelFiltro_FiltroId",
                table: "UserModelFiltro",
                column: "FiltroId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserModelFiltro");

            migrationBuilder.CreateTable(
                name: "savedFolder",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FiltroId = table.Column<long>(type: "bigint", nullable: false),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_savedFolder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_savedFolder_Filtro_FiltroId",
                        column: x => x.FiltroId,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_savedFolder_FiltroId",
                table: "savedFolder",
                column: "FiltroId");
        }
    }
}
