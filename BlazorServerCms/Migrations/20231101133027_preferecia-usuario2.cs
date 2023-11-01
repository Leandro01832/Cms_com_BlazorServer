using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class prefereciausuario2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPreferences",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    capitulo = table.Column<int>(type: "int", nullable: false),
                    pasta = table.Column<int>(type: "int", nullable: false),
                    p1 = table.Column<int>(type: "int", nullable: false),
                    p2 = table.Column<int>(type: "int", nullable: false),
                    p3 = table.Column<int>(type: "int", nullable: false),
                    p4 = table.Column<int>(type: "int", nullable: false),
                    p5 = table.Column<int>(type: "int", nullable: false),
                    p6 = table.Column<int>(type: "int", nullable: false),
                    p7 = table.Column<int>(type: "int", nullable: false),
                    p8 = table.Column<int>(type: "int", nullable: false),
                    p9 = table.Column<int>(type: "int", nullable: false),
                    p10 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPreferences");
        }
    }
}
