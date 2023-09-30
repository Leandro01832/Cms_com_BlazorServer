using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Migrations
{
    public partial class userbooks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBook10",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LivroId = table.Column<long>(type: "bigint", nullable: false),
                    Capitulo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBook10", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBook10_Livro_LivroId",
                        column: x => x.LivroId,
                        principalTable: "Livro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBook2",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LivroId = table.Column<long>(type: "bigint", nullable: false),
                    Capitulo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBook2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBook2_Livro_LivroId",
                        column: x => x.LivroId,
                        principalTable: "Livro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBook3",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LivroId = table.Column<long>(type: "bigint", nullable: false),
                    Capitulo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBook3", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBook3_Livro_LivroId",
                        column: x => x.LivroId,
                        principalTable: "Livro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBook4",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LivroId = table.Column<long>(type: "bigint", nullable: false),
                    Capitulo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBook4", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBook4_Livro_LivroId",
                        column: x => x.LivroId,
                        principalTable: "Livro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBook5",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LivroId = table.Column<long>(type: "bigint", nullable: false),
                    Capitulo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBook5", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBook5_Livro_LivroId",
                        column: x => x.LivroId,
                        principalTable: "Livro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBook6",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LivroId = table.Column<long>(type: "bigint", nullable: false),
                    Capitulo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBook6", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBook6_Livro_LivroId",
                        column: x => x.LivroId,
                        principalTable: "Livro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBook7",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LivroId = table.Column<long>(type: "bigint", nullable: false),
                    Capitulo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBook7", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBook7_Livro_LivroId",
                        column: x => x.LivroId,
                        principalTable: "Livro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBook8",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LivroId = table.Column<long>(type: "bigint", nullable: false),
                    Capitulo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBook8", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBook8_Livro_LivroId",
                        column: x => x.LivroId,
                        principalTable: "Livro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBook9",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LivroId = table.Column<long>(type: "bigint", nullable: false),
                    Capitulo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBook9", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBook9_Livro_LivroId",
                        column: x => x.LivroId,
                        principalTable: "Livro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBook10_LivroId",
                table: "UserBook10",
                column: "LivroId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBook2_LivroId",
                table: "UserBook2",
                column: "LivroId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBook3_LivroId",
                table: "UserBook3",
                column: "LivroId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBook4_LivroId",
                table: "UserBook4",
                column: "LivroId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBook5_LivroId",
                table: "UserBook5",
                column: "LivroId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBook6_LivroId",
                table: "UserBook6",
                column: "LivroId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBook7_LivroId",
                table: "UserBook7",
                column: "LivroId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBook8_LivroId",
                table: "UserBook8",
                column: "LivroId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBook9_LivroId",
                table: "UserBook9",
                column: "LivroId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBook10");

            migrationBuilder.DropTable(
                name: "UserBook2");

            migrationBuilder.DropTable(
                name: "UserBook3");

            migrationBuilder.DropTable(
                name: "UserBook4");

            migrationBuilder.DropTable(
                name: "UserBook5");

            migrationBuilder.DropTable(
                name: "UserBook6");

            migrationBuilder.DropTable(
                name: "UserBook7");

            migrationBuilder.DropTable(
                name: "UserBook8");

            migrationBuilder.DropTable(
                name: "UserBook9");
        }
    }
}
