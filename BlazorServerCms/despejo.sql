IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [HashUserName] nvarchar(max) NULL,
    [Compartilhar] nvarchar(max) NULL,
    [UpdateShare] bit NOT NULL,
    [Image] nvarchar(max) NULL,
    [PontosPorDia] int NOT NULL,
    [DataPontuacao] datetime2 NOT NULL,
    [Recorde] int NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [Cliente] (
    [Id] bigint NOT NULL IDENTITY,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [UserName] nvarchar(max) NOT NULL,
    [Cpf] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Cliente] PRIMARY KEY ([Id])
);

CREATE TABLE [Compartilhante] (
    [Id] bigint NOT NULL IDENTITY,
    [Data] datetime2 NOT NULL,
    [Livro] nvarchar(max) NULL,
    [Comissao] int NOT NULL,
    [CupomDesconto] nvarchar(max) NULL,
    CONSTRAINT [PK_Compartilhante] PRIMARY KEY ([Id])
);

CREATE TABLE [Produto] (
    [Id] bigint NOT NULL IDENTITY,
    [Descricao] nvarchar(max) NULL,
    [Nome] nvarchar(max) NULL,
    [Preco] decimal(18,2) NOT NULL,
    [QuantEstoque] int NOT NULL,
    CONSTRAINT [PK_Produto] PRIMARY KEY ([Id])
);

CREATE TABLE [Rota] (
    [Id] bigint NOT NULL IDENTITY,
    [Nome] nvarchar(max) NULL,
    [Registrado] bit NOT NULL,
    CONSTRAINT [PK_Rota] PRIMARY KEY ([Id])
);

CREATE TABLE [Story] (
    [Id] bigint NOT NULL IDENTITY,
    [Modelo] int NOT NULL,
    [Nome] nvarchar(max) NULL,
    [Image] nvarchar(max) NULL,
    [Descricao] nvarchar(max) NULL,
    [Capitulo] int NOT NULL,
    [Discriminator] nvarchar(13) NOT NULL,
    CONSTRAINT [PK_Story] PRIMARY KEY ([Id])
);

CREATE TABLE [Time] (
    [Id] bigint NOT NULL IDENTITY,
    [nome] nvarchar(450) NULL,
    [vendas] int NOT NULL,
    CONSTRAINT [PK_Time] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Assinatura] (
    [Id] bigint NOT NULL IDENTITY,
    [UserModelId] nvarchar(450) NULL,
    CONSTRAINT [PK_Assinatura] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Assinatura_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id])
);

CREATE TABLE [Hashtag] (
    [Id] bigint NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [UserModelId] nvarchar(450) NULL,
    CONSTRAINT [PK_Hashtag] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Hashtag_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id])
);

CREATE TABLE [Endereco] (
    [Id] bigint NOT NULL,
    [Estado] nvarchar(max) NOT NULL,
    [Cidade] nvarchar(max) NOT NULL,
    [Bairro] nvarchar(max) NOT NULL,
    [Rua] nvarchar(max) NOT NULL,
    [Numero] bigint NOT NULL,
    [Cep] nvarchar(max) NOT NULL,
    [Complemento] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Endereco] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Endereco_Cliente_Id] FOREIGN KEY ([Id]) REFERENCES [Cliente] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Pedido] (
    [Id] bigint NOT NULL IDENTITY,
    [ClienteId] bigint NOT NULL,
    [Status] nvarchar(max) NULL,
    CONSTRAINT [PK_Pedido] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Pedido_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Cliente] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Telefone] (
    [Id] bigint NOT NULL,
    [DDD_Celular] nvarchar(max) NOT NULL,
    [Celular] nvarchar(max) NOT NULL,
    [DDD_Telefone] nvarchar(max) NULL,
    [Fone] nvarchar(max) NULL,
    CONSTRAINT [PK_Telefone] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Telefone_Cliente_Id] FOREIGN KEY ([Id]) REFERENCES [Cliente] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ImagemProduto] (
    [Id] bigint NOT NULL IDENTITY,
    [ProdutoId] bigint NOT NULL,
    [ArquivoImagem] nvarchar(max) NULL,
    [WidthImagem] int NOT NULL,
    CONSTRAINT [PK_ImagemProduto] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ImagemProduto_Produto_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [Produto] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserModelTime] (
    [UserModelId] nvarchar(450) NOT NULL,
    [TimeId] bigint NOT NULL,
    CONSTRAINT [PK_UserModelTime] PRIMARY KEY ([UserModelId], [TimeId]),
    CONSTRAINT [FK_UserModelTime_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserModelTime_Time_TimeId] FOREIGN KEY ([TimeId]) REFERENCES [Time] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Livro] (
    [Id] bigint NOT NULL,
    [Nome] nvarchar(max) NOT NULL,
    [Capa] nvarchar(max) NOT NULL,
    [BookNumber] int NOT NULL,
    [StandardChapter] int NOT NULL,
    [url] nvarchar(max) NULL,
    CONSTRAINT [PK_Livro] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Livro_Assinatura_Id] FOREIGN KEY ([Id]) REFERENCES [Assinatura] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ItemPedido] (
    [Id] bigint NOT NULL IDENTITY,
    [Quantidade] int NOT NULL,
    [ProdutoId] bigint NOT NULL,
    [PedidoId] bigint NOT NULL,
    [PrecoUnitario] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_ItemPedido] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ItemPedido_Pedido_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [Pedido] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ItemPedido_Produto_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [Produto] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Anotacao] (
    [Id] bigint NOT NULL IDENTITY,
    [DataCriacao] datetime2 NOT NULL,
    [LivroId] bigint NULL,
    [Capitulo] int NOT NULL,
    [Query] nvarchar(max) NOT NULL,
    [UserModelId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Anotacao] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Anotacao_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Anotacao_Livro_LivroId] FOREIGN KEY ([LivroId]) REFERENCES [Livro] ([Id])
);

CREATE TABLE [Camada] (
    [Id] bigint NOT NULL IDENTITY,
    [Numero] int NOT NULL,
    [LivroId] bigint NULL,
    CONSTRAINT [PK_Camada] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Camada_Livro_LivroId] FOREIGN KEY ([LivroId]) REFERENCES [Livro] ([Id])
);

CREATE TABLE [Content] (
    [Id] bigint NOT NULL IDENTITY,
    [Data] datetime2 NOT NULL,
    [Titulo] nvarchar(max) NOT NULL,
    [StoryId] bigint NOT NULL,
    [LivroId] bigint NULL,
    [Rotas] nvarchar(max) NULL,
    [QuantLiked] int NOT NULL,
    [QuantShared] int NOT NULL,
    [Html] nvarchar(max) NULL,
    [Discriminator] nvarchar(21) NOT NULL,
    [Versiculo] int NULL,
    [Posicao] int NULL,
    [UserModelId] nvarchar(450) NULL,
    [ContentId] bigint NULL,
    CONSTRAINT [PK_Content] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Content_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Content_Content_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Content] ([Id]),
    CONSTRAINT [FK_Content_Livro_LivroId] FOREIGN KEY ([LivroId]) REFERENCES [Livro] ([Id]),
    CONSTRAINT [FK_Content_Story_StoryId] FOREIGN KEY ([StoryId]) REFERENCES [Story] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserModelLivro] (
    [UserModelId] nvarchar(450) NOT NULL,
    [LivroId] bigint NOT NULL,
    CONSTRAINT [PK_UserModelLivro] PRIMARY KEY ([UserModelId], [LivroId]),
    CONSTRAINT [FK_UserModelLivro_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserModelLivro_Livro_LivroId] FOREIGN KEY ([LivroId]) REFERENCES [Livro] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Criterio] (
    [Id] bigint NOT NULL,
    [CamadaId] bigint NOT NULL,
    [Descricao] nvarchar(max) NOT NULL,
    [DataCriacao] datetime2 NOT NULL,
    [LivroId] bigint NULL,
    [Ativo] bit NOT NULL,
    CONSTRAINT [PK_Criterio] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Criterio_Camada_CamadaId] FOREIGN KEY ([CamadaId]) REFERENCES [Camada] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Criterio_Content_Id] FOREIGN KEY ([Id]) REFERENCES [Content] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Criterio_Livro_LivroId] FOREIGN KEY ([LivroId]) REFERENCES [Livro] ([Id])
);

CREATE TABLE [HashtagContent] (
    [HashtagId] bigint NOT NULL,
    [ContentId] bigint NOT NULL,
    CONSTRAINT [PK_HashtagContent] PRIMARY KEY ([HashtagId], [ContentId]),
    CONSTRAINT [FK_HashtagContent_Content_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Content] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_HashtagContent_Hashtag_HashtagId] FOREIGN KEY ([HashtagId]) REFERENCES [Hashtag] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [MarcacaoVideoFilter] (
    [Id] bigint NOT NULL IDENTITY,
    [VideoFilterId] bigint NOT NULL,
    [Segundos] int NOT NULL,
    CONSTRAINT [PK_MarcacaoVideoFilter] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MarcacaoVideoFilter_Content_VideoFilterId] FOREIGN KEY ([VideoFilterId]) REFERENCES [Content] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [MudancaEstado] (
    [Id] bigint NOT NULL,
    [Pontos] int NOT NULL,
    [Curtidas] bigint NOT NULL,
    [Compartilhamentos] bigint NOT NULL,
    [Type] nvarchar(max) NULL,
    [IdContent] bigint NOT NULL,
    CONSTRAINT [PK_MudancaEstado] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MudancaEstado_Content_Id] FOREIGN KEY ([Id]) REFERENCES [Content] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ProdutoConteudo] (
    [ProdutoId] bigint NOT NULL,
    [ContentId] bigint NOT NULL,
    CONSTRAINT [PK_ProdutoConteudo] PRIMARY KEY ([ProdutoId], [ContentId]),
    CONSTRAINT [FK_ProdutoConteudo_Content_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Content] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProdutoConteudo_Produto_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [Produto] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserModelPageLiked] (
    [UserModelId] nvarchar(450) NOT NULL,
    [ContentId] bigint NOT NULL,
    CONSTRAINT [PK_UserModelPageLiked] PRIMARY KEY ([UserModelId], [ContentId]),
    CONSTRAINT [FK_UserModelPageLiked_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserModelPageLiked_Content_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Content] ([Id])
);

CREATE TABLE [Filtro] (
    [Id] bigint NOT NULL IDENTITY,
    [Nome] nvarchar(max) NULL,
    [Rotas] nvarchar(max) NULL,
    [CamadaId] bigint NULL,
    [StoryId] bigint NOT NULL,
    [LivroId] bigint NULL,
    [CriterioId] bigint NULL,
    [VetorEmbedding] varbinary(max) NULL,
    [Discriminator] nvarchar(13) NOT NULL,
    [ComCriterio] bigint NULL,
    [FiltroId] bigint NULL,
    [UltimaPasta] bit NULL,
    CONSTRAINT [PK_Filtro] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Filtro_Camada_CamadaId] FOREIGN KEY ([CamadaId]) REFERENCES [Camada] ([Id]),
    CONSTRAINT [FK_Filtro_Criterio_CriterioId] FOREIGN KEY ([CriterioId]) REFERENCES [Criterio] ([Id]),
    CONSTRAINT [FK_Filtro_Filtro_FiltroId] FOREIGN KEY ([FiltroId]) REFERENCES [Filtro] ([Id]),
    CONSTRAINT [FK_Filtro_Livro_LivroId] FOREIGN KEY ([LivroId]) REFERENCES [Livro] ([Id]),
    CONSTRAINT [FK_Filtro_Story_StoryId] FOREIGN KEY ([StoryId]) REFERENCES [Story] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AnotacaoVersiculo] (
    [FiltroId] bigint NOT NULL,
    [AnotacaoId] bigint NOT NULL,
    [Id] bigint NOT NULL,
    CONSTRAINT [PK_AnotacaoVersiculo] PRIMARY KEY ([AnotacaoId], [FiltroId]),
    CONSTRAINT [FK_AnotacaoVersiculo_Anotacao_AnotacaoId] FOREIGN KEY ([AnotacaoId]) REFERENCES [Anotacao] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AnotacaoVersiculo_Filtro_FiltroId] FOREIGN KEY ([FiltroId]) REFERENCES [Filtro] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [FiltroContent] (
    [ContentId] bigint NOT NULL,
    [FiltroId] bigint NOT NULL,
    CONSTRAINT [PK_FiltroContent] PRIMARY KEY ([FiltroId], [ContentId]),
    CONSTRAINT [FK_FiltroContent_Content_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Content] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_FiltroContent_Filtro_FiltroId] FOREIGN KEY ([FiltroId]) REFERENCES [Filtro] ([Id])
);

CREATE TABLE [HashtagFiltro] (
    [HashtagId] bigint NOT NULL,
    [FiltroId] bigint NOT NULL,
    CONSTRAINT [PK_HashtagFiltro] PRIMARY KEY ([HashtagId], [FiltroId]),
    CONSTRAINT [FK_HashtagFiltro_Filtro_FiltroId] FOREIGN KEY ([FiltroId]) REFERENCES [Filtro] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_HashtagFiltro_Hashtag_HashtagId] FOREIGN KEY ([HashtagId]) REFERENCES [Hashtag] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [PastaSalva] (
    [Id] bigint NOT NULL,
    CONSTRAINT [PK_PastaSalva] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PastaSalva_Filtro_Id] FOREIGN KEY ([Id]) REFERENCES [Filtro] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserModelFiltro] (
    [UserModelId] nvarchar(450) NOT NULL,
    [FiltroId] bigint NOT NULL,
    CONSTRAINT [PK_UserModelFiltro] PRIMARY KEY ([UserModelId], [FiltroId]),
    CONSTRAINT [FK_UserModelFiltro_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserModelFiltro_Filtro_FiltroId] FOREIGN KEY ([FiltroId]) REFERENCES [Filtro] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserModelPastaSalva] (
    [UserModelId] nvarchar(450) NOT NULL,
    [PastaSalvaId] bigint NOT NULL,
    CONSTRAINT [PK_UserModelPastaSalva] PRIMARY KEY ([UserModelId], [PastaSalvaId]),
    CONSTRAINT [FK_UserModelPastaSalva_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserModelPastaSalva_PastaSalva_PastaSalvaId] FOREIGN KEY ([PastaSalvaId]) REFERENCES [PastaSalva] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Anotacao_LivroId] ON [Anotacao] ([LivroId]);

CREATE INDEX [IX_Anotacao_UserModelId] ON [Anotacao] ([UserModelId]);

CREATE INDEX [IX_AnotacaoVersiculo_FiltroId] ON [AnotacaoVersiculo] ([FiltroId]);

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

CREATE INDEX [IX_Assinatura_UserModelId] ON [Assinatura] ([UserModelId]);

CREATE INDEX [IX_Camada_LivroId] ON [Camada] ([LivroId]);

CREATE INDEX [IX_Content_ContentId] ON [Content] ([ContentId]);

CREATE INDEX [IX_Content_LivroId] ON [Content] ([LivroId]);

CREATE INDEX [IX_Content_StoryId] ON [Content] ([StoryId]);

CREATE INDEX [IX_Content_UserModelId] ON [Content] ([UserModelId]);

CREATE INDEX [IX_Criterio_CamadaId] ON [Criterio] ([CamadaId]);

CREATE INDEX [IX_Criterio_LivroId] ON [Criterio] ([LivroId]);

CREATE INDEX [IX_Filtro_CamadaId] ON [Filtro] ([CamadaId]);

CREATE INDEX [IX_Filtro_CriterioId] ON [Filtro] ([CriterioId]);

CREATE INDEX [IX_Filtro_FiltroId] ON [Filtro] ([FiltroId]);

CREATE INDEX [IX_Filtro_LivroId] ON [Filtro] ([LivroId]);

CREATE INDEX [IX_Filtro_StoryId] ON [Filtro] ([StoryId]);

CREATE INDEX [IX_FiltroContent_ContentId] ON [FiltroContent] ([ContentId]);

CREATE INDEX [IX_Hashtag_UserModelId] ON [Hashtag] ([UserModelId]);

CREATE INDEX [IX_HashtagContent_ContentId] ON [HashtagContent] ([ContentId]);

CREATE INDEX [IX_HashtagFiltro_FiltroId] ON [HashtagFiltro] ([FiltroId]);

CREATE INDEX [IX_ImagemProduto_ProdutoId] ON [ImagemProduto] ([ProdutoId]);

CREATE INDEX [IX_ItemPedido_PedidoId] ON [ItemPedido] ([PedidoId]);

CREATE INDEX [IX_ItemPedido_ProdutoId] ON [ItemPedido] ([ProdutoId]);

CREATE INDEX [IX_MarcacaoVideoFilter_VideoFilterId] ON [MarcacaoVideoFilter] ([VideoFilterId]);

CREATE INDEX [IX_Pedido_ClienteId] ON [Pedido] ([ClienteId]);

CREATE INDEX [IX_ProdutoConteudo_ContentId] ON [ProdutoConteudo] ([ContentId]);

CREATE UNIQUE INDEX [IX_Time_nome] ON [Time] ([nome]) WHERE [nome] IS NOT NULL;

CREATE INDEX [IX_UserModelFiltro_FiltroId] ON [UserModelFiltro] ([FiltroId]);

CREATE INDEX [IX_UserModelLivro_LivroId] ON [UserModelLivro] ([LivroId]);

CREATE INDEX [IX_UserModelPageLiked_ContentId] ON [UserModelPageLiked] ([ContentId]);

CREATE INDEX [IX_UserModelPastaSalva_PastaSalvaId] ON [UserModelPastaSalva] ([PastaSalvaId]);

CREATE INDEX [IX_UserModelTime_TimeId] ON [UserModelTime] ([TimeId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260510120524_Inicio', N'9.0.11');

DROP TABLE [Compartilhante];

DECLARE @var sysname;
SELECT @var = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Story]') AND [c].[name] = N'Discriminator');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Story] DROP CONSTRAINT [' + @var + '];');
ALTER TABLE [Story] DROP COLUMN [Discriminator];

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Story]') AND [c].[name] = N'Modelo');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Story] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Story] DROP COLUMN [Modelo];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260601212848_removerProp', N'9.0.11');

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'UpdateShare');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [UpdateShare];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260601213948_removerpropuser', N'9.0.11');

ALTER TABLE [Filtro] DROP CONSTRAINT [FK_Filtro_Filtro_FiltroId];

CREATE TABLE [UserPasskey] (
    [Id] bigint NOT NULL IDENTITY,
    [UserModelId] nvarchar(450) NULL,
    [CredentialId] nvarchar(max) NOT NULL,
    [PublicKey] varbinary(max) NOT NULL,
    [Counter] bigint NOT NULL,
    CONSTRAINT [PK_UserPasskey] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserPasskey_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id])
);

CREATE INDEX [IX_UserPasskey_UserModelId] ON [UserPasskey] ([UserModelId]);

ALTER TABLE [Filtro] ADD CONSTRAINT [FK_Filtro_Filtro_FiltroId] FOREIGN KEY ([FiltroId]) REFERENCES [Filtro] ([Id]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260607215503_UserPass', N'9.0.11');

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserPasskey]') AND [c].[name] = N'PublicKey');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [UserPasskey] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [UserPasskey] ALTER COLUMN [PublicKey] nvarchar(max) NOT NULL;

ALTER TABLE [UserModelFiltro] ADD [Data] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [AspNetUsers] ADD [Decorar] int NOT NULL DEFAULT 0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260608160611_decorar', N'9.0.11');

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserModelFiltro]') AND [c].[name] = N'Data');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [UserModelFiltro] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [UserModelFiltro] DROP COLUMN [Data];

ALTER TABLE [HashtagFiltro] ADD [Data] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [HashtagContent] ADD [Data] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609164730_dataUserModel', N'9.0.11');

DROP TABLE [UserModelPastaSalva];

DROP TABLE [PastaSalva];

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HashtagFiltro]') AND [c].[name] = N'Data');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [HashtagFiltro] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [HashtagFiltro] DROP COLUMN [Data];

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HashtagContent]') AND [c].[name] = N'Data');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [HashtagContent] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [HashtagContent] DROP COLUMN [Data];

CREATE TABLE [Relogio] (
    [Id] bigint NOT NULL IDENTITY,
    [SubFiltroId] bigint NOT NULL,
    [ContentId] bigint NOT NULL,
    [UserModelId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Relogio] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Relogio_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Relogio_Content_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Content] ([Id]),
    CONSTRAINT [FK_Relogio_Filtro_SubFiltroId] FOREIGN KEY ([SubFiltroId]) REFERENCES [Filtro] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Relogio_ContentId] ON [Relogio] ([ContentId]);

CREATE INDEX [IX_Relogio_SubFiltroId] ON [Relogio] ([SubFiltroId]);

CREATE INDEX [IX_Relogio_UserModelId] ON [Relogio] ([UserModelId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609234310_relogioUsuario', N'9.0.11');

CREATE TABLE [RelogioParede] (
    [Id] bigint NOT NULL,
    [SubFiltroId] bigint NOT NULL,
    [ContentId] bigint NOT NULL,
    [UserModelId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_RelogioParede] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RelogioParede_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RelogioParede_Content_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Content] ([Id]),
    CONSTRAINT [FK_RelogioParede_Filtro_SubFiltroId] FOREIGN KEY ([SubFiltroId]) REFERENCES [Filtro] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RelogioParede_Time_Id] FOREIGN KEY ([Id]) REFERENCES [Time] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_RelogioParede_ContentId] ON [RelogioParede] ([ContentId]);

CREATE INDEX [IX_RelogioParede_SubFiltroId] ON [RelogioParede] ([SubFiltroId]);

CREATE INDEX [IX_RelogioParede_UserModelId] ON [RelogioParede] ([UserModelId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260611145407_relogioParedeTime', N'9.0.11');

ALTER TABLE [RelogioParede] ADD [Data] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

ALTER TABLE [Relogio] ADD [Data] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260612175736_DataRelogio', N'9.0.11');

ALTER TABLE [AspNetUsers] ADD [TipoBaralho] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260708002732_tiposContent', N'9.0.11');

ALTER TABLE [Filtro] ADD [Embaralhar] bit NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260708135231_embaralhar', N'9.0.11');

ALTER TABLE [MarcacaoVideoFilter] DROP CONSTRAINT [FK_MarcacaoVideoFilter_Content_VideoFilterId];

DROP INDEX [IX_MarcacaoVideoFilter_VideoFilterId] ON [MarcacaoVideoFilter];

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MarcacaoVideoFilter]') AND [c].[name] = N'VideoFilterId');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [MarcacaoVideoFilter] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [MarcacaoVideoFilter] DROP COLUMN [VideoFilterId];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260709131120_videoFilter', N'9.0.11');

ALTER TABLE [MarcacaoVideoFilter] ADD [ContentId] bigint NOT NULL DEFAULT CAST(0 AS bigint);

CREATE INDEX [IX_MarcacaoVideoFilter_ContentId] ON [MarcacaoVideoFilter] ([ContentId]);

ALTER TABLE [MarcacaoVideoFilter] ADD CONSTRAINT [FK_MarcacaoVideoFilter_Content_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Content] ([Id]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260709135247_marcacao', N'9.0.11');

ALTER TABLE [Content] DROP CONSTRAINT [FK_Content_Content_ContentId];

DROP INDEX [IX_Content_ContentId] ON [Content];

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Content]') AND [c].[name] = N'ContentId');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Content] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [Content] DROP COLUMN [ContentId];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260715104440_comentar', N'9.0.11');

ALTER TABLE [Content] ADD [ContentId] bigint NULL;

ALTER TABLE [Content] ADD [ContentId1] bigint NULL;

CREATE INDEX [IX_Content_ContentId] ON [Content] ([ContentId]);

CREATE INDEX [IX_Content_ContentId1] ON [Content] ([ContentId1]);

ALTER TABLE [Content] ADD CONSTRAINT [FK_Content_Content_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Content] ([Id]);

ALTER TABLE [Content] ADD CONSTRAINT [FK_Content_Content_ContentId1] FOREIGN KEY ([ContentId1]) REFERENCES [Content] ([Id]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260715111200_comentar2', N'9.0.11');

ALTER TABLE [Content] DROP CONSTRAINT [FK_Content_AspNetUsers_UserModelId];

DROP INDEX [IX_Content_UserModelId] ON [Content];

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Content]') AND [c].[name] = N'UserModelId');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Content] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [Content] DROP COLUMN [UserModelId];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260715122001_userContent', N'9.0.11');

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Content]') AND [c].[name] = N'Posicao');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Content] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [Content] DROP COLUMN [Posicao];

ALTER TABLE [Content] ADD [UserModelId] nvarchar(450) NULL;

CREATE INDEX [IX_Content_UserModelId] ON [Content] ([UserModelId]);

ALTER TABLE [Content] ADD CONSTRAINT [FK_Content_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260715170721_usuarioContent', N'9.0.11');

ALTER TABLE [Content] DROP CONSTRAINT [FK_Content_Content_ContentId];

ALTER TABLE [Content] DROP CONSTRAINT [FK_Content_Content_ContentId1];

DROP INDEX [IX_Content_ContentId] ON [Content];

DROP INDEX [IX_Content_ContentId1] ON [Content];

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Content]') AND [c].[name] = N'ContentId');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Content] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [Content] DROP COLUMN [ContentId];

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Content]') AND [c].[name] = N'ContentId1');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Content] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [Content] DROP COLUMN [ContentId1];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260715173722_usuarioContent2', N'9.0.11');

CREATE TABLE [Comment] (
    [Id] bigint NOT NULL IDENTITY,
    [Comentar] nvarchar(max) NULL,
    [ContentId] bigint NULL,
    [UserModelId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Comment] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Comment_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Comment_Content_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Content] ([Id])
);

CREATE INDEX [IX_Comment_ContentId] ON [Comment] ([ContentId]);

CREATE INDEX [IX_Comment_UserModelId] ON [Comment] ([UserModelId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260715184815_usuarioContent3', N'9.0.11');

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Content]') AND [c].[name] = N'Rotas');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Content] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [Content] DROP COLUMN [Rotas];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260715222207_removeprop', N'9.0.11');

ALTER TABLE [Comment] DROP CONSTRAINT [FK_Comment_AspNetUsers_UserModelId];

ALTER TABLE [Comment] DROP CONSTRAINT [FK_Comment_Content_ContentId];

ALTER TABLE [Comment] DROP CONSTRAINT [PK_Comment];

EXEC sp_rename N'[Comment]', N'Comments', 'OBJECT';

EXEC sp_rename N'[Comments].[IX_Comment_UserModelId]', N'IX_Comments_UserModelId', 'INDEX';

EXEC sp_rename N'[Comments].[IX_Comment_ContentId]', N'IX_Comments_ContentId', 'INDEX';

ALTER TABLE [Comments] ADD CONSTRAINT [PK_Comments] PRIMARY KEY ([Id]);

ALTER TABLE [Comments] ADD CONSTRAINT [FK_Comments_AspNetUsers_UserModelId] FOREIGN KEY ([UserModelId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;

ALTER TABLE [Comments] ADD CONSTRAINT [FK_Comments_Content_ContentId] FOREIGN KEY ([ContentId]) REFERENCES [Content] ([Id]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260720181843_CorrigindoHerancaComment', N'9.0.11');

CREATE TABLE [UserFollow] (
    [ObserverId] nvarchar(450) NOT NULL,
    [TargetId] nvarchar(450) NOT NULL,
    [FollowedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_UserFollow] PRIMARY KEY ([ObserverId], [TargetId]),
    CONSTRAINT [FK_UserFollow_AspNetUsers_ObserverId] FOREIGN KEY ([ObserverId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UserFollow_AspNetUsers_TargetId] FOREIGN KEY ([TargetId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_UserFollow_TargetId] ON [UserFollow] ([TargetId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260720214023_following', N'9.0.11');

DROP TABLE [UserModelFiltro];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260721230810_removeusermodelFiltro', N'9.0.11');

ALTER TABLE [RelogioParede] ADD [HashtagId] bigint NULL;

ALTER TABLE [Relogio] ADD [HashtagId] bigint NULL;

CREATE INDEX [IX_RelogioParede_HashtagId] ON [RelogioParede] ([HashtagId]);

CREATE INDEX [IX_Relogio_HashtagId] ON [Relogio] ([HashtagId]);

ALTER TABLE [Relogio] ADD CONSTRAINT [FK_Relogio_Hashtag_HashtagId] FOREIGN KEY ([HashtagId]) REFERENCES [Hashtag] ([Id]);

ALTER TABLE [RelogioParede] ADD CONSTRAINT [FK_RelogioParede_Hashtag_HashtagId] FOREIGN KEY ([HashtagId]) REFERENCES [Hashtag] ([Id]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260724205521_hashtagRelogio', N'9.0.11');

ALTER TABLE [HashtagContent] ADD [Data] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260724230013_dataHashtagContent', N'9.0.11');

EXEC sp_rename N'[AspNetUsers].[HashUserName]', N'Nome', 'COLUMN';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260811233208_removerHashUserName', N'9.0.11');

COMMIT;
GO

