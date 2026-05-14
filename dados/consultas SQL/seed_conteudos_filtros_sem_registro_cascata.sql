DECLARE @Videos TABLE (
    Ordem int NOT NULL,
    VideoId varchar(20) NOT NULL
);

INSERT INTO @Videos (Ordem, VideoId)
VALUES
    (0, 'r_eLOG096sE'),
    (1, 'ui70_rhmGtM'),
    (2, '1zueHDxHJWA'),
    (3, '_qRM-gmgMXY'),
    (4, 'tIafILj-GEs'),
    (5, '_TOKrAiFw-Q'),
    (6, 'BHdyUaVz3Os'),
    (7, 'zKcPVi43gqo'),
    (8, 'OSJRgjzWdxw'),
    (9, 'X5qca5AR0T4'),
    (10, '7nvIBiiIq_8'),
    (11, '56lw2Arku0g'),
    (12, 'AhJX4W96LII'),
    (13, 'ZaWxtXYx6U');

DECLARE @FiltrosSemRegistro TABLE (
    Ordem int IDENTITY(1,1) NOT NULL,
    FiltroId bigint NOT NULL,
    Nome nvarchar(255) NOT NULL,
    CamadaId bigint NULL,
    StoryId bigint NOT NULL,
    LivroId bigint NULL
);

INSERT INTO @FiltrosSemRegistro (FiltroId, Nome, CamadaId, StoryId, LivroId)
SELECT f.Id, f.Nome, f.CamadaId, f.StoryId, f.LivroId
FROM Filtro f
WHERE NOT EXISTS (
    SELECT 1
    FROM FiltroContent fc
    WHERE fc.FiltroId = f.Id
)
ORDER BY f.CamadaId, f.Id;

DECLARE @Ordem int;
DECLARE @FiltroId bigint;
DECLARE @Nome nvarchar(255);
DECLARE @CamadaId bigint;
DECLARE @StoryId bigint;
DECLARE @LivroId bigint;
DECLARE @ContentId bigint;
DECLARE @Rota nvarchar(300);
DECLARE @Titulo nvarchar(300);
DECLARE @Html nvarchar(max);
DECLARE @Formato int;
DECLARE @VideoId varchar(20);
DECLARE @ImagemUrl nvarchar(600);

DECLARE filtros_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT Ordem, FiltroId, Nome, CamadaId, StoryId, LivroId
    FROM @FiltrosSemRegistro
    ORDER BY Ordem;

OPEN filtros_cursor;
FETCH NEXT FROM filtros_cursor INTO @Ordem, @FiltroId, @Nome, @CamadaId, @StoryId, @LivroId;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @ContentId = NULL;
    SET @Formato = @Ordem % 5;
    SET @Rota = CONCAT('/natureza/conteudo-filtro-', @FiltroId);
    SET @Titulo = CONCAT(N'Guia - ', @Nome);
    SET @ImagemUrl = CONCAT(N'https://picsum.photos/seed/cms-filtro-sem-registro-', @FiltroId, N'/512/384');

    SELECT @VideoId = VideoId
    FROM @Videos
    WHERE Ordem = @Ordem % (SELECT COUNT(*) FROM @Videos);

    SET @Html =
        CASE @Formato
            WHEN 0 THEN CONCAT(
                N'<h1>', @Nome, N'</h1>',
                N'<p><img style="display: block; margin-left: auto; margin-right: auto;" src="', @ImagemUrl, N'" alt="', @Nome, N'" width="512" height="384" /></p>',
                N'<p>', @Nome, N' pertence a camada ', COALESCE(CONVERT(nvarchar(20), @CamadaId), N''), N' da classificacao. Esta pagina serve como introducao visual para o filtro.</p>',
                N'<p>Use este conteudo como base para adicionar exemplos, imagens reais e observacoes especificas.</p>'
            )
            WHEN 1 THEN CONCAT(
                N'<h1>', @Nome, N'</h1>',
                N'<p style="text-align: center;"><iframe title="Video sobre ', @Nome, N'" src="https://www.youtube.com/embed/', @VideoId, N'" width="320" height="560" frameborder="0" allowfullscreen="allowfullscreen"></iframe></p>',
                N'<p>Video inicial para complementar o filtro ', @Nome, N'. Substitua por um video mais especifico quando desejar refinar o conteudo.</p>'
            )
            WHEN 2 THEN CONCAT(
                N'<h1>', @Nome, N'</h1>',
                N'<p><strong>Resumo:</strong> ', @Nome, N' e um filtro usado para organizar conteudos da story Natureza.</p>',
                N'<h2>O que abordar</h2>',
                N'<ul>',
                N'<li>Caracteristicas principais.</li>',
                N'<li>Exemplos conhecidos.</li>',
                N'<li>Ambiente, comportamento ou funcao ecologica.</li>',
                N'<li>Curiosidades para facilitar a identificacao.</li>',
                N'</ul>'
            )
            WHEN 3 THEN CONCAT(
                N'<h1>', @Nome, N'</h1>',
                N'<p>Este conteudo foi criado para evitar que o filtro fique vazio no CMS.</p>',
                N'<table class="table"><tbody>',
                N'<tr><th>Filtro</th><td>', @Nome, N'</td></tr>',
                N'<tr><th>Camada</th><td>', COALESCE(CONVERT(nvarchar(20), @CamadaId), N''), N'</td></tr>',
                N'<tr><th>Uso sugerido</th><td>Adicionar descricao, midia e exemplos relacionados.</td></tr>',
                N'</tbody></table>'
            )
            ELSE CONCAT(
                N'<h1>', @Nome, N'</h1>',
                N'<p>', @Nome, N' agora possui uma pagina inicial vinculada ao filtro e aos seus filtros superiores em cascata.</p>',
                N'<p><img style="display: block; margin-left: auto; margin-right: auto;" src="', @ImagemUrl, N'" alt="', @Nome, N'" width="320" height="240" /></p>',
                N'<p style="text-align: center;"><iframe title="Referencia em video para ', @Nome, N'" src="https://www.youtube.com/embed/', @VideoId, N'" width="320" height="560" frameborder="0" allowfullscreen="allowfullscreen"></iframe></p>'
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
            StoryId = @StoryId,
            LivroId = @LivroId,
            Html = @Html,
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

    FETCH NEXT FROM filtros_cursor INTO @Ordem, @FiltroId, @Nome, @CamadaId, @StoryId, @LivroId;
END;

CLOSE filtros_cursor;
DEALLOCATE filtros_cursor;

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
