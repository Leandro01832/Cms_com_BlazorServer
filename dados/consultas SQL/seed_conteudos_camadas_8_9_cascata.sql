DECLARE @FiltroId bigint;
DECLARE @Nome nvarchar(255);
DECLARE @StoryId bigint;
DECLARE @LivroId bigint;
DECLARE @ContentId bigint;
DECLARE @Rota nvarchar(300);
DECLARE @Titulo nvarchar(300);
DECLARE @Html nvarchar(max);
DECLARE @Indice int = 0;
DECLARE @Formato int;
DECLARE @VideoId varchar(20);
DECLARE @ImagemUrl nvarchar(600);

DECLARE filtros_camadas_finais CURSOR LOCAL FAST_FORWARD FOR
    SELECT f.Id, f.Nome, f.StoryId, f.LivroId
    FROM Filtro f
    WHERE f.CamadaId >= 8
      AND NOT EXISTS (
          SELECT 1
          FROM FiltroContent fc
          WHERE fc.FiltroId = f.Id
      )
    ORDER BY f.CamadaId, f.Id;

OPEN filtros_camadas_finais;
FETCH NEXT FROM filtros_camadas_finais INTO @FiltroId, @Nome, @StoryId, @LivroId;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @ContentId = NULL;
    SET @Indice = @Indice + 1;
    SET @Formato = @Indice % 3;
    SET @Rota = CONCAT('/natureza/filtro-', @FiltroId);
    SET @Titulo = CONCAT(N'Conteudo - ', @Nome);

    SET @ImagemUrl = CONCAT(N'https://picsum.photos/seed/cms-filtro-', @FiltroId, N'/512/384');
    SET @VideoId =
        CASE @Indice % 8
            WHEN 0 THEN 'r_eLOG096sE'
            WHEN 1 THEN 'ui70_rhmGtM'
            WHEN 2 THEN '1zueHDxHJWA'
            WHEN 3 THEN '_qRM-gmgMXY'
            WHEN 4 THEN 'tIafILj-GEs'
            WHEN 5 THEN '_TOKrAiFw-Q'
            WHEN 6 THEN 'BHdyUaVz3Os'
            ELSE 'zKcPVi43gqo'
        END;

    SET @Html =
        CASE @Formato
            WHEN 0 THEN CONCAT(
                N'<h1>', @Nome, N'</h1>',
                N'<p><img style="display: block; margin-left: auto; margin-right: auto;" src="', @ImagemUrl, N'" alt="', @Nome, N'" width="512" height="384" /></p>',
                N'<p>', @Nome, N' faz parte das classificacoes finais da story Natureza. Este conteudo pode ser usado como capa visual para apresentar especies, grupos, exemplos e curiosidades.</p>',
                N'<p>Complete esta pagina com uma descricao propria, imagens reais e observacoes importantes para o seu CMS.</p>'
            )
            WHEN 1 THEN CONCAT(
                N'<h1>', @Nome, N'</h1>',
                N'<p style="text-align: center;"><iframe title="Video sobre ', @Nome, N'" src="https://www.youtube.com/embed/', @VideoId, N'" width="320" height="560" frameborder="0" allowfullscreen="allowfullscreen"></iframe></p>',
                N'<p>Este video serve como referencia inicial para enriquecer o filtro ', @Nome, N'. Troque o link quando quiser usar um material especifico.</p>'
            )
            ELSE CONCAT(
                N'<h1>', @Nome, N'</h1>',
                N'<p><strong>Resumo:</strong> ', @Nome, N' representa um grupo de classificacao em camada final. A pagina pode explicar caracteristicas, exemplos conhecidos, habitat, comportamento e importancia ecologica.</p>',
                N'<h2>Pontos para desenvolver</h2>',
                N'<ul>',
                N'<li>Caracteristicas principais do grupo.</li>',
                N'<li>Exemplos comuns ou especies relacionadas.</li>',
                N'<li>Ambientes onde costuma aparecer.</li>',
                N'<li>Curiosidades para ajudar o leitor a reconhecer o tema.</li>',
                N'</ul>',
                N'<p>Esse texto foi criado como base editorial para evitar paginas vazias nas ultimas camadas do CMS.</p>'
            )
        END;

    SELECT @ContentId = c.Id
    FROM Content c
    WHERE c.Rotas = @Rota;

    IF @ContentId IS NULL
    BEGIN
        INSERT INTO Content
            (Data, Titulo, StoryId, LivroId, Rotas, QuantLiked, QuantShared, Html, Discriminator, Versiculo, Posicao, UserModelId, ContentId)
        VALUES
            (SYSDATETIME(), @Titulo, @StoryId, @LivroId, @Rota, 0, 0, @Html, 'Pagina', 0, NULL, NULL, NULL);

        SET @ContentId = SCOPE_IDENTITY();
    END;
    ELSE
    BEGIN
        UPDATE Content
        SET Titulo = @Titulo,
            Html = @Html,
            StoryId = @StoryId,
            LivroId = @LivroId,
            Discriminator = 'Pagina'
        WHERE Id = @ContentId;
    END;

    ;WITH FiltroCascata AS (
        SELECT Id, FiltroId, CamadaId
        FROM Filtro
        WHERE Id = @FiltroId

        UNION ALL

        SELECT pai.Id, pai.FiltroId, pai.CamadaId
        FROM Filtro pai
        INNER JOIN FiltroCascata filho ON pai.Id = filho.FiltroId
        WHERE filho.FiltroId IS NOT NULL
    )
    INSERT INTO FiltroContent (ContentId, FiltroId)
    SELECT @ContentId, fc.Id
    FROM FiltroCascata fc
    WHERE NOT EXISTS (
        SELECT 1
        FROM FiltroContent existente
        WHERE existente.ContentId = @ContentId
          AND existente.FiltroId = fc.Id
    )
    OPTION (MAXRECURSION 20);

    FETCH NEXT FROM filtros_camadas_finais INTO @FiltroId, @Nome, @StoryId, @LivroId;
END;

CLOSE filtros_camadas_finais;
DEALLOCATE filtros_camadas_finais;

-- Conferencia de cobertura por camada depois da execucao.
SELECT
    f.CamadaId,
    COUNT(*) AS TotalFiltros,
    SUM(CASE WHEN EXISTS (
        SELECT 1
        FROM FiltroContent fc
        WHERE fc.FiltroId = f.Id
    ) THEN 1 ELSE 0 END) AS FiltrosComConteudo,
    SUM(CASE WHEN NOT EXISTS (
        SELECT 1
        FROM FiltroContent fc
        WHERE fc.FiltroId = f.Id
    ) THEN 1 ELSE 0 END) AS FiltrosSemConteudo
FROM Filtro f
GROUP BY f.CamadaId
ORDER BY f.CamadaId;
