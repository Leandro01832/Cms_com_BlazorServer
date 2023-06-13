using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServerCms.Data.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comentario",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPagina = table.Column<long>(type: "bigint", nullable: false),
                    Capitulo = table.Column<int>(type: "int", nullable: false),
                    Verso = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Compartilhamento",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Livro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Capitulo = table.Column<int>(type: "int", nullable: false),
                    Verso = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compartilhamento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Container",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    BorderRadius = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    FlexWrap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JustifyContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlexDirection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlignItems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlignSelf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassesModificadoras = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Padding = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Container", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Div",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Padding = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<bool>(type: "bit", nullable: false),
                    BorderRadius = table.Column<int>(type: "int", nullable: false),
                    Pagina_ = table.Column<long>(type: "bigint", nullable: false),
                    FlexWrap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JustifyContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlexDirection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlignItems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlignSelf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassesModificadoras = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Div", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Livro",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Capitulo = table.Column<int>(type: "int", nullable: false),
                    Compartilhando = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livro", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Story",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comentario = table.Column<bool>(type: "bit", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaginaPadraoLink = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Story", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Telefone",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DDD_Celular = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Celular = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DDD_Telefone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClienteId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telefone", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Texto",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PalavrasTexto = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Texto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoIncorporado",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tamanho = table.Column<int>(type: "int", nullable: true),
                    ArquivoVideoIncorporado = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoIncorporado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DivContainer",
                columns: table => new
                {
                    ContainerId = table.Column<long>(type: "bigint", nullable: false),
                    DivId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DivContainer", x => new { x.DivId, x.ContainerId });
                    table.ForeignKey(
                        name: "FK_DivContainer_Container_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Container",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DivContainer_Div_DivId",
                        column: x => x.DivId,
                        principalTable: "Div",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Filtro",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rotas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoryId = table.Column<long>(type: "bigint", nullable: false),
                    SubStoryId = table.Column<long>(type: "bigint", nullable: true),
                    GrupoId = table.Column<long>(type: "bigint", nullable: true),
                    SubGrupoId = table.Column<long>(type: "bigint", nullable: true),
                    SubSubGrupoId = table.Column<long>(type: "bigint", nullable: true),
                    CamadaSeisId = table.Column<long>(type: "bigint", nullable: true),
                    CamadaSeteId = table.Column<long>(type: "bigint", nullable: true),
                    CamadaOitoId = table.Column<long>(type: "bigint", nullable: true),
                    CamadaNoveId = table.Column<long>(type: "bigint", nullable: true),
                    CamadaDezId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filtro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Filtro_Story_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Story",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DivElemento",
                columns: table => new
                {
                    ElementoId = table.Column<long>(type: "bigint", nullable: false),
                    DivId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DivElemento", x => new { x.DivId, x.ElementoId });
                    table.ForeignKey(
                        name: "FK_DivElemento_Div_DivId",
                        column: x => x.DivId,
                        principalTable: "Div",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DivElemento_Texto_ElementoId",
                        column: x => x.ElementoId,
                        principalTable: "Texto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubStory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubStory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubStory_Filtro_Id",
                        column: x => x.Id,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubStory_Story_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Story",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Grupo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubStoryId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grupo_Filtro_Id",
                        column: x => x.Id,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grupo_SubStory_SubStoryId",
                        column: x => x.SubStoryId,
                        principalTable: "SubStory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SubGrupo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrupoId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubGrupo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubGrupo_Filtro_Id",
                        column: x => x.Id,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubGrupo_Grupo_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SubSubGrupo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubGrupoId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubSubGrupo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubSubGrupo_Filtro_Id",
                        column: x => x.Id,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubSubGrupo_SubGrupo_SubGrupoId",
                        column: x => x.SubGrupoId,
                        principalTable: "SubGrupo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "CamadaSeis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubSubGrupoId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CamadaSeis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CamadaSeis_Filtro_Id",
                        column: x => x.Id,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CamadaSeis_SubSubGrupo_SubSubGrupoId",
                        column: x => x.SubSubGrupoId,
                        principalTable: "SubSubGrupo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "CamadaSete",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CamadaSeisId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CamadaSete", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CamadaSete_CamadaSeis_CamadaSeisId",
                        column: x => x.CamadaSeisId,
                        principalTable: "CamadaSeis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CamadaSete_Filtro_Id",
                        column: x => x.Id,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "CamadaOito",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CamadaSeteId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CamadaOito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CamadaOito_CamadaSete_CamadaSeteId",
                        column: x => x.CamadaSeteId,
                        principalTable: "CamadaSete",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CamadaOito_Filtro_Id",
                        column: x => x.Id,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "CamadaNove",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CamadaOitoId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CamadaNove", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CamadaNove_CamadaOito_CamadaOitoId",
                        column: x => x.CamadaOitoId,
                        principalTable: "CamadaOito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CamadaNove_Filtro_Id",
                        column: x => x.Id,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "CamadaDez",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CamadaNoveId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CamadaDez", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CamadaDez_CamadaNove_CamadaNoveId",
                        column: x => x.CamadaNoveId,
                        principalTable: "CamadaNove",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CamadaDez_Filtro_Id",
                        column: x => x.Id,
                        principalTable: "Filtro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Pagina",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoryId = table.Column<long>(type: "bigint", nullable: false),
                    SubStoryId = table.Column<long>(type: "bigint", nullable: true),
                    GrupoId = table.Column<long>(type: "bigint", nullable: true),
                    SubGrupoId = table.Column<long>(type: "bigint", nullable: true),
                    SubSubGrupoId = table.Column<long>(type: "bigint", nullable: true),
                    CamadaSeisId = table.Column<long>(type: "bigint", nullable: true),
                    CamadaSeteId = table.Column<long>(type: "bigint", nullable: true),
                    CamadaOitoId = table.Column<long>(type: "bigint", nullable: true),
                    CamadaNoveId = table.Column<long>(type: "bigint", nullable: true),
                    CamadaDezId = table.Column<long>(type: "bigint", nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sobreescrita = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArquivoMusic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Music = table.Column<bool>(type: "bit", nullable: false),
                    Comentario = table.Column<long>(type: "bigint", nullable: true),
                    ImagemContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Layout = table.Column<bool>(type: "bit", nullable: false),
                    FlexDirection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlignItems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tempo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagina_CamadaDez_CamadaDezId",
                        column: x => x.CamadaDezId,
                        principalTable: "CamadaDez",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagina_CamadaNove_CamadaNoveId",
                        column: x => x.CamadaNoveId,
                        principalTable: "CamadaNove",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagina_CamadaOito_CamadaOitoId",
                        column: x => x.CamadaOitoId,
                        principalTable: "CamadaOito",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagina_CamadaSeis_CamadaSeisId",
                        column: x => x.CamadaSeisId,
                        principalTable: "CamadaSeis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagina_CamadaSete_CamadaSeteId",
                        column: x => x.CamadaSeteId,
                        principalTable: "CamadaSete",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagina_Grupo_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "Grupo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagina_Story_StoryId",
                        column: x => x.StoryId,
                        principalTable: "Story",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pagina_SubGrupo_SubGrupoId",
                        column: x => x.SubGrupoId,
                        principalTable: "SubGrupo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagina_SubStory_SubStoryId",
                        column: x => x.SubStoryId,
                        principalTable: "SubStory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pagina_SubSubGrupo_SubSubGrupoId",
                        column: x => x.SubSubGrupoId,
                        principalTable: "SubSubGrupo",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaginaContainer",
                columns: table => new
                {
                    ContainerId = table.Column<long>(type: "bigint", nullable: false),
                    PaginaId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaginaContainer", x => new { x.ContainerId, x.PaginaId });
                    table.ForeignKey(
                        name: "FK_PaginaContainer_Container_ContainerId",
                        column: x => x.ContainerId,
                        principalTable: "Container",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaginaContainer_Pagina_PaginaId",
                        column: x => x.PaginaId,
                        principalTable: "Pagina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantEstoque = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produto_Pagina_Id",
                        column: x => x.Id,
                        principalTable: "Pagina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImagemProduto",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<long>(type: "bigint", nullable: false),
                    ArquivoImagem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WidthImagem = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagemProduto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImagemProduto_Produto_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemPedido",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<long>(type: "bigint", nullable: false),
                    PedidoId = table.Column<long>(type: "bigint", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemPedido_Pedido_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemPedido_Produto_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CamadaDez_CamadaNoveId",
                table: "CamadaDez",
                column: "CamadaNoveId");

            migrationBuilder.CreateIndex(
                name: "IX_CamadaNove_CamadaOitoId",
                table: "CamadaNove",
                column: "CamadaOitoId");

            migrationBuilder.CreateIndex(
                name: "IX_CamadaOito_CamadaSeteId",
                table: "CamadaOito",
                column: "CamadaSeteId");

            migrationBuilder.CreateIndex(
                name: "IX_CamadaSeis_SubSubGrupoId",
                table: "CamadaSeis",
                column: "SubSubGrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_CamadaSete_CamadaSeisId",
                table: "CamadaSete",
                column: "CamadaSeisId");

            migrationBuilder.CreateIndex(
                name: "IX_DivContainer_ContainerId",
                table: "DivContainer",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_DivElemento_ElementoId",
                table: "DivElemento",
                column: "ElementoId");

            migrationBuilder.CreateIndex(
                name: "IX_Filtro_StoryId",
                table: "Filtro",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Grupo_SubStoryId",
                table: "Grupo",
                column: "SubStoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImagemProduto_ProdutoId",
                table: "ImagemProduto",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPedido_PedidoId",
                table: "ItemPedido",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPedido_ProdutoId",
                table: "ItemPedido",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagina_CamadaDezId",
                table: "Pagina",
                column: "CamadaDezId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagina_CamadaNoveId",
                table: "Pagina",
                column: "CamadaNoveId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagina_CamadaOitoId",
                table: "Pagina",
                column: "CamadaOitoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagina_CamadaSeisId",
                table: "Pagina",
                column: "CamadaSeisId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagina_CamadaSeteId",
                table: "Pagina",
                column: "CamadaSeteId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagina_GrupoId",
                table: "Pagina",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagina_StoryId",
                table: "Pagina",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagina_SubGrupoId",
                table: "Pagina",
                column: "SubGrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagina_SubStoryId",
                table: "Pagina",
                column: "SubStoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagina_SubSubGrupoId",
                table: "Pagina",
                column: "SubSubGrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_PaginaContainer_PaginaId",
                table: "PaginaContainer",
                column: "PaginaId");

            migrationBuilder.CreateIndex(
                name: "IX_SubGrupo_GrupoId",
                table: "SubGrupo",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_SubStory_StoryId",
                table: "SubStory",
                column: "StoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubSubGrupo_SubGrupoId",
                table: "SubSubGrupo",
                column: "SubGrupoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comentario");

            migrationBuilder.DropTable(
                name: "Compartilhamento");

            migrationBuilder.DropTable(
                name: "DivContainer");

            migrationBuilder.DropTable(
                name: "DivElemento");

            migrationBuilder.DropTable(
                name: "ImagemProduto");

            migrationBuilder.DropTable(
                name: "ItemPedido");

            migrationBuilder.DropTable(
                name: "Livro");

            migrationBuilder.DropTable(
                name: "PaginaContainer");

            migrationBuilder.DropTable(
                name: "Telefone");

            migrationBuilder.DropTable(
                name: "VideoIncorporado");

            migrationBuilder.DropTable(
                name: "Div");

            migrationBuilder.DropTable(
                name: "Texto");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "Container");

            migrationBuilder.DropTable(
                name: "Pagina");

            migrationBuilder.DropTable(
                name: "CamadaDez");

            migrationBuilder.DropTable(
                name: "CamadaNove");

            migrationBuilder.DropTable(
                name: "CamadaOito");

            migrationBuilder.DropTable(
                name: "CamadaSete");

            migrationBuilder.DropTable(
                name: "CamadaSeis");

            migrationBuilder.DropTable(
                name: "SubSubGrupo");

            migrationBuilder.DropTable(
                name: "SubGrupo");

            migrationBuilder.DropTable(
                name: "Grupo");

            migrationBuilder.DropTable(
                name: "SubStory");

            migrationBuilder.DropTable(
                name: "Filtro");

            migrationBuilder.DropTable(
                name: "Story");
        }
    }
}
