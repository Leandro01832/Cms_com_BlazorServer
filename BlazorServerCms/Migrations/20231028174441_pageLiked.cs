using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class pageLiked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoIncorporado");

            migrationBuilder.CreateTable(
                name: "PageLiked",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    capitulo = table.Column<int>(type: "int", nullable: false),
                    indice = table.Column<int>(type: "int", nullable: false),
                    verso = table.Column<int>(type: "int", nullable: false),
                    substory = table.Column<int>(type: "int", nullable: false),
                    grupo = table.Column<int>(type: "int", nullable: true),
                    subgrupo = table.Column<int>(type: "int", nullable: true),
                    subsubgrupo = table.Column<int>(type: "int", nullable: true),
                    camadaSeis = table.Column<int>(type: "int", nullable: true),
                    camadaSete = table.Column<int>(type: "int", nullable: true),
                    camadaOito = table.Column<int>(type: "int", nullable: true),
                    camadaNove = table.Column<int>(type: "int", nullable: true),
                    camadaDez = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageLiked", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageLiked");

            migrationBuilder.CreateTable(
                name: "VideoIncorporado",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArquivoVideoIncorporado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tamanho = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoIncorporado", x => x.Id);
                });
        }
    }
}
