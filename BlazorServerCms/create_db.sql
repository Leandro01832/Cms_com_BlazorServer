CREATE TABLE "AspNetRoles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY,
    "Name" TEXT NULL,
    "NormalizedName" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL
);


CREATE TABLE "AspNetUsers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,
    "HashUserName" TEXT NULL,
    "Compartilhar" TEXT NULL,
    "UpdateShare" INTEGER NOT NULL,
    "Image" TEXT NULL,
    "PontosPorDia" INTEGER NOT NULL,
    "DataPontuacao" TEXT NOT NULL,
    "Recorde" INTEGER NOT NULL,
    "UserName" TEXT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email" TEXT NULL,
    "NormalizedEmail" TEXT NULL,
    "EmailConfirmed" INTEGER NOT NULL,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL,
    "TwoFactorEnabled" INTEGER NOT NULL,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL
);


CREATE TABLE "Cliente" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Cliente" PRIMARY KEY AUTOINCREMENT,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "UserName" TEXT NOT NULL,
    "Cpf" TEXT NOT NULL
);


CREATE TABLE "Compartilhante" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Compartilhante" PRIMARY KEY AUTOINCREMENT,
    "Data" TEXT NOT NULL,
    "Livro" TEXT NULL,
    "Comissao" INTEGER NOT NULL,
    "CupomDesconto" TEXT NULL
);


CREATE TABLE "Produto" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Produto" PRIMARY KEY AUTOINCREMENT,
    "Descricao" TEXT NULL,
    "Nome" TEXT NULL,
    "Preco" TEXT NOT NULL,
    "QuantEstoque" INTEGER NOT NULL
);


CREATE TABLE "Rota" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Rota" PRIMARY KEY AUTOINCREMENT,
    "Nome" TEXT NULL,
    "Registrado" INTEGER NOT NULL
);


CREATE TABLE "Story" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Story" PRIMARY KEY AUTOINCREMENT,
    "Modelo" INTEGER NOT NULL,
    "Nome" TEXT NULL,
    "Image" TEXT NULL,
    "Descricao" TEXT NULL,
    "Capitulo" INTEGER NOT NULL,
    "Discriminator" TEXT NOT NULL
);


CREATE TABLE "Time" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Time" PRIMARY KEY AUTOINCREMENT,
    "nome" TEXT NULL,
    "vendas" INTEGER NOT NULL
);


CREATE TABLE "AspNetRoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Assinatura" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Assinatura" PRIMARY KEY AUTOINCREMENT,
    "UserModelId" TEXT NULL,
    CONSTRAINT "FK_Assinatura_AspNetUsers_UserModelId" FOREIGN KEY ("UserModelId") REFERENCES "AspNetUsers" ("Id")
);


CREATE TABLE "Endereco" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Endereco" PRIMARY KEY,
    "Estado" TEXT NOT NULL,
    "Cidade" TEXT NOT NULL,
    "Bairro" TEXT NOT NULL,
    "Rua" TEXT NOT NULL,
    "Numero" INTEGER NOT NULL,
    "Cep" TEXT NOT NULL,
    "Complemento" TEXT NOT NULL,
    CONSTRAINT "FK_Endereco_Cliente_Id" FOREIGN KEY ("Id") REFERENCES "Cliente" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Pedido" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Pedido" PRIMARY KEY AUTOINCREMENT,
    "ClienteId" INTEGER NOT NULL,
    "Status" TEXT NULL,
    CONSTRAINT "FK_Pedido_Cliente_ClienteId" FOREIGN KEY ("ClienteId") REFERENCES "Cliente" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Telefone" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Telefone" PRIMARY KEY,
    "DDD_Celular" TEXT NOT NULL,
    "Celular" TEXT NOT NULL,
    "DDD_Telefone" TEXT NULL,
    "Fone" TEXT NULL,
    CONSTRAINT "FK_Telefone_Cliente_Id" FOREIGN KEY ("Id") REFERENCES "Cliente" ("Id") ON DELETE CASCADE
);


CREATE TABLE "ImagemProduto" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ImagemProduto" PRIMARY KEY AUTOINCREMENT,
    "ProdutoId" INTEGER NOT NULL,
    "ArquivoImagem" TEXT NULL,
    "WidthImagem" INTEGER NOT NULL,
    CONSTRAINT "FK_ImagemProduto_Produto_ProdutoId" FOREIGN KEY ("ProdutoId") REFERENCES "Produto" ("Id") ON DELETE CASCADE
);


CREATE TABLE "UserModelTime" (
    "UserModelId" TEXT NOT NULL,
    "TimeId" INTEGER NOT NULL,
    CONSTRAINT "PK_UserModelTime" PRIMARY KEY ("UserModelId", "TimeId"),
    CONSTRAINT "FK_UserModelTime_AspNetUsers_UserModelId" FOREIGN KEY ("UserModelId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UserModelTime_Time_TimeId" FOREIGN KEY ("TimeId") REFERENCES "Time" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Livro" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Livro" PRIMARY KEY,
    "Nome" TEXT NOT NULL,
    "Capa" TEXT NOT NULL,
    "BookNumber" INTEGER NOT NULL,
    "StandardChapter" INTEGER NOT NULL,
    "url" TEXT NULL,
    CONSTRAINT "FK_Livro_Assinatura_Id" FOREIGN KEY ("Id") REFERENCES "Assinatura" ("Id") ON DELETE CASCADE
);


CREATE TABLE "ItemPedido" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ItemPedido" PRIMARY KEY AUTOINCREMENT,
    "Quantidade" INTEGER NOT NULL,
    "ProdutoId" INTEGER NOT NULL,
    "PedidoId" INTEGER NOT NULL,
    "PrecoUnitario" TEXT NOT NULL,
    CONSTRAINT "FK_ItemPedido_Pedido_PedidoId" FOREIGN KEY ("PedidoId") REFERENCES "Pedido" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ItemPedido_Produto_ProdutoId" FOREIGN KEY ("ProdutoId") REFERENCES "Produto" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Camada" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Camada" PRIMARY KEY AUTOINCREMENT,
    "Numero" INTEGER NOT NULL,
    "LivroId" INTEGER NULL,
    CONSTRAINT "FK_Camada_Livro_LivroId" FOREIGN KEY ("LivroId") REFERENCES "Livro" ("Id")
);


CREATE TABLE "Content" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Content" PRIMARY KEY AUTOINCREMENT,
    "Data" TEXT NOT NULL,
    "Titulo" TEXT NOT NULL,
    "StoryId" INTEGER NOT NULL,
    "LivroId" INTEGER NULL,
    "Rotas" TEXT NULL,
    "QuantLiked" INTEGER NOT NULL,
    "QuantShared" INTEGER NOT NULL,
    "Html" TEXT NULL,
    "Discriminator" TEXT NOT NULL,
    "Versiculo" INTEGER NULL,
    "Posicao" INTEGER NULL,
    "UserModelId" TEXT NULL,
    "ContentId" INTEGER NULL,
    CONSTRAINT "FK_Content_AspNetUsers_UserModelId" FOREIGN KEY ("UserModelId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Content_Content_ContentId" FOREIGN KEY ("ContentId") REFERENCES "Content" ("Id"),
    CONSTRAINT "FK_Content_Livro_LivroId" FOREIGN KEY ("LivroId") REFERENCES "Livro" ("Id"),
    CONSTRAINT "FK_Content_Story_StoryId" FOREIGN KEY ("StoryId") REFERENCES "Story" ("Id") ON DELETE CASCADE
);


CREATE TABLE "UserModelLivro" (
    "UserModelId" TEXT NOT NULL,
    "LivroId" INTEGER NOT NULL,
    CONSTRAINT "PK_UserModelLivro" PRIMARY KEY ("UserModelId", "LivroId"),
    CONSTRAINT "FK_UserModelLivro_AspNetUsers_UserModelId" FOREIGN KEY ("UserModelId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UserModelLivro_Livro_LivroId" FOREIGN KEY ("LivroId") REFERENCES "Livro" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Criterio" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Criterio" PRIMARY KEY,
    "CamadaId" INTEGER NOT NULL,
    "Descricao" TEXT NOT NULL,
    "DataCriacao" TEXT NOT NULL,
    "LivroId" INTEGER NULL,
    "Ativo" INTEGER NOT NULL,
    CONSTRAINT "FK_Criterio_Camada_CamadaId" FOREIGN KEY ("CamadaId") REFERENCES "Camada" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Criterio_Content_Id" FOREIGN KEY ("Id") REFERENCES "Content" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Criterio_Livro_LivroId" FOREIGN KEY ("LivroId") REFERENCES "Livro" ("Id")
);


CREATE TABLE "MarcacaoVideoFilter" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_MarcacaoVideoFilter" PRIMARY KEY AUTOINCREMENT,
    "VideoFilterId" INTEGER NOT NULL,
    "Segundos" INTEGER NOT NULL,
    CONSTRAINT "FK_MarcacaoVideoFilter_Content_VideoFilterId" FOREIGN KEY ("VideoFilterId") REFERENCES "Content" ("Id") ON DELETE CASCADE
);


CREATE TABLE "MudancaEstado" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_MudancaEstado" PRIMARY KEY,
    "Pontos" INTEGER NOT NULL,
    "Curtidas" INTEGER NOT NULL,
    "Compartilhamentos" INTEGER NOT NULL,
    "Type" TEXT NULL,
    "IdContent" INTEGER NOT NULL,
    CONSTRAINT "FK_MudancaEstado_Content_Id" FOREIGN KEY ("Id") REFERENCES "Content" ("Id") ON DELETE CASCADE
);


CREATE TABLE "ProdutoConteudo" (
    "ProdutoId" INTEGER NOT NULL,
    "ContentId" INTEGER NOT NULL,
    "Id" INTEGER NOT NULL,
    CONSTRAINT "PK_ProdutoConteudo" PRIMARY KEY ("ProdutoId", "ContentId"),
    CONSTRAINT "FK_ProdutoConteudo_Content_ContentId" FOREIGN KEY ("ContentId") REFERENCES "Content" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProdutoConteudo_Produto_ProdutoId" FOREIGN KEY ("ProdutoId") REFERENCES "Produto" ("Id") ON DELETE CASCADE
);


CREATE TABLE "UserModelPageLiked" (
    "UserModelId" TEXT NOT NULL,
    "ContentId" INTEGER NOT NULL,
    CONSTRAINT "PK_UserModelPageLiked" PRIMARY KEY ("UserModelId", "ContentId"),
    CONSTRAINT "FK_UserModelPageLiked_AspNetUsers_UserModelId" FOREIGN KEY ("UserModelId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UserModelPageLiked_Content_ContentId" FOREIGN KEY ("ContentId") REFERENCES "Content" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Filtro" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Filtro" PRIMARY KEY AUTOINCREMENT,
    "Nome" TEXT NULL,
    "Rotas" TEXT NULL,
    "CamadaId" INTEGER NULL,
    "StoryId" INTEGER NOT NULL,
    "LivroId" INTEGER NULL,
    "CriterioId" INTEGER NULL,
    "Discriminator" TEXT NOT NULL,
    "ComCriterio" INTEGER NULL,
    "FiltroId" INTEGER NULL,
    CONSTRAINT "FK_Filtro_Camada_CamadaId" FOREIGN KEY ("CamadaId") REFERENCES "Camada" ("Id"),
    CONSTRAINT "FK_Filtro_Criterio_CriterioId" FOREIGN KEY ("CriterioId") REFERENCES "Criterio" ("Id"),
    CONSTRAINT "FK_Filtro_Filtro_FiltroId" FOREIGN KEY ("FiltroId") REFERENCES "Filtro" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Filtro_Livro_LivroId" FOREIGN KEY ("LivroId") REFERENCES "Livro" ("Id"),
    CONSTRAINT "FK_Filtro_Story_StoryId" FOREIGN KEY ("StoryId") REFERENCES "Story" ("Id") ON DELETE CASCADE
);


CREATE TABLE "FiltroContent" (
    "ContentId" INTEGER NOT NULL,
    "FiltroId" INTEGER NOT NULL,
    CONSTRAINT "PK_FiltroContent" PRIMARY KEY ("FiltroId", "ContentId"),
    CONSTRAINT "FK_FiltroContent_Content_ContentId" FOREIGN KEY ("ContentId") REFERENCES "Content" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_FiltroContent_Filtro_FiltroId" FOREIGN KEY ("FiltroId") REFERENCES "Filtro" ("Id") ON DELETE CASCADE
);


CREATE TABLE "PastaSalva" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_PastaSalva" PRIMARY KEY,
    CONSTRAINT "FK_PastaSalva_Filtro_Id" FOREIGN KEY ("Id") REFERENCES "Filtro" ("Id") ON DELETE CASCADE
);


CREATE TABLE "UserModelFiltro" (
    "UserModelId" TEXT NOT NULL,
    "FiltroId" INTEGER NOT NULL,
    CONSTRAINT "PK_UserModelFiltro" PRIMARY KEY ("UserModelId", "FiltroId"),
    CONSTRAINT "FK_UserModelFiltro_AspNetUsers_UserModelId" FOREIGN KEY ("UserModelId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UserModelFiltro_Filtro_FiltroId" FOREIGN KEY ("FiltroId") REFERENCES "Filtro" ("Id") ON DELETE CASCADE
);


CREATE TABLE "UserModelPastaSalva" (
    "UserModelId" TEXT NOT NULL,
    "PastaSalvaId" INTEGER NOT NULL,
    CONSTRAINT "PK_UserModelPastaSalva" PRIMARY KEY ("UserModelId", "PastaSalvaId"),
    CONSTRAINT "FK_UserModelPastaSalva_AspNetUsers_UserModelId" FOREIGN KEY ("UserModelId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UserModelPastaSalva_PastaSalva_PastaSalvaId" FOREIGN KEY ("PastaSalvaId") REFERENCES "PastaSalva" ("Id") ON DELETE CASCADE
);


CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");


CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");


CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");


CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");


CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");


CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");


CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");


CREATE INDEX "IX_Assinatura_UserModelId" ON "Assinatura" ("UserModelId");


CREATE INDEX "IX_Camada_LivroId" ON "Camada" ("LivroId");


CREATE INDEX "IX_Content_ContentId" ON "Content" ("ContentId");


CREATE INDEX "IX_Content_LivroId" ON "Content" ("LivroId");


CREATE INDEX "IX_Content_StoryId" ON "Content" ("StoryId");


CREATE INDEX "IX_Content_UserModelId" ON "Content" ("UserModelId");


CREATE INDEX "IX_Criterio_CamadaId" ON "Criterio" ("CamadaId");


CREATE INDEX "IX_Criterio_LivroId" ON "Criterio" ("LivroId");


CREATE INDEX "IX_Filtro_CamadaId" ON "Filtro" ("CamadaId");


CREATE INDEX "IX_Filtro_CriterioId" ON "Filtro" ("CriterioId");


CREATE INDEX "IX_Filtro_FiltroId" ON "Filtro" ("FiltroId");


CREATE INDEX "IX_Filtro_LivroId" ON "Filtro" ("LivroId");


CREATE INDEX "IX_Filtro_StoryId" ON "Filtro" ("StoryId");


CREATE INDEX "IX_FiltroContent_ContentId" ON "FiltroContent" ("ContentId");


CREATE INDEX "IX_ImagemProduto_ProdutoId" ON "ImagemProduto" ("ProdutoId");


CREATE INDEX "IX_ItemPedido_PedidoId" ON "ItemPedido" ("PedidoId");


CREATE INDEX "IX_ItemPedido_ProdutoId" ON "ItemPedido" ("ProdutoId");


CREATE INDEX "IX_MarcacaoVideoFilter_VideoFilterId" ON "MarcacaoVideoFilter" ("VideoFilterId");


CREATE INDEX "IX_Pedido_ClienteId" ON "Pedido" ("ClienteId");


CREATE INDEX "IX_ProdutoConteudo_ContentId" ON "ProdutoConteudo" ("ContentId");


CREATE UNIQUE INDEX "IX_Time_nome" ON "Time" ("nome");


CREATE INDEX "IX_UserModelFiltro_FiltroId" ON "UserModelFiltro" ("FiltroId");


CREATE INDEX "IX_UserModelLivro_LivroId" ON "UserModelLivro" ("LivroId");


CREATE INDEX "IX_UserModelPageLiked_ContentId" ON "UserModelPageLiked" ("ContentId");


CREATE INDEX "IX_UserModelPastaSalva_PastaSalvaId" ON "UserModelPastaSalva" ("PastaSalvaId");


CREATE INDEX "IX_UserModelTime_TimeId" ON "UserModelTime" ("TimeId");


