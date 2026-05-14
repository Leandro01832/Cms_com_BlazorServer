DECLARE @Videos TABLE (
    VideoId varchar(20) NOT NULL,
    FiltroId bigint NOT NULL,
    Titulo nvarchar(300) NOT NULL,
    Texto nvarchar(max) NOT NULL
);

INSERT INTO @Videos (VideoId, FiltroId, Titulo, Texto)
VALUES
    ('OSJRgjzWdxw', 20093, N'Hylidae: pererecas e diversidade', N'Video para apresentar a familia Hylidae, grupo de anfibios muito associado a pererecas arboricolas.'),
    ('_78juYRABYo', 20093, N'Hylidae: registro curto', N'Video curto para complementar a pagina de Hylidae com uma referencia visual rapida.'),
    ('wSxmyVLnbBc', 20092, N'Bufonidae: sapos verdadeiros', N'Conteudo em video para introduzir a familia Bufonidae e suas caracteristicas gerais.'),
    ('ZEsBcKTWT7M', 20092, N'Bufonidae: curiosidades', N'Video complementar para enriquecer o filtro Bufonidae com exemplos e observacoes.'),
    ('X5qca5AR0T4', 20116, N'Viperidae: serpentes peconhentas', N'Video para contextualizar a familia Viperidae dentro dos repteis.'),
    ('iGdIm9m8EAs', 20117, N'Boidae: jiboias e parentes', N'Video para apresentar a familia Boidae e seus representantes mais conhecidos.'),
    ('7nvIBiiIq_8', 20121, N'Cheloniidae: tartarugas marinhas', N'Video introdutorio para a familia Cheloniidae, grupo de tartarugas marinhas.'),
    ('56lw2Arku0g', 20077, N'Potamotrygonidae: raias de agua doce', N'Video para apresentar raias de agua doce da familia Potamotrygonidae.'),
    ('KN1cP7h-Jro', 20068, N'Loricariidae: cascudos', N'Video para introduzir a familia Loricariidae, conhecida por incluir muitos cascudos.'),
    ('7sFCl-OQ3r4', 20068, N'Loricariidae: exemplos visuais', N'Video complementar para a pagina de Loricariidae.'),
    ('VNK-D-2NRYc', 20066, N'Serrasalmidae: piranhas e parentes', N'Video para contextualizar a familia Serrasalmidae em peixes de agua doce.'),
    ('D1ePbgBJldg', 20063, N'Characidae: tetras e lambaris', N'Video introdutorio para a familia Characidae.'),
    ('GfZm_rQh5Nc', 20063, N'Characidae: diversidade', N'Video complementar para mostrar variedade dentro de Characidae.'),
    ('vbCJLCZob3U', 20073, N'Scombridae: atuns e cavalas', N'Video para apresentar a familia Scombridae e seus peixes nadadores.'),
    ('AhJX4W96LII', 20141, N'Trochilidae: beija-flores', N'Video para a familia Trochilidae, grupo dos beija-flores.'),
    ('gmzxPQkc-aI', 20141, N'Trochilidae: voo e alimentacao', N'Video complementar para observar comportamento de beija-flores.'),
    ('ZaWxtXYx6U', 20139, N'Psittacidae: araras e papagaios', N'Video para apresentar a familia Psittacidae.'),
    ('r_YPiy24mFc', 20139, N'Psittacidae: comportamento', N'Video complementar sobre aves da familia Psittacidae.'),
    ('ZYRSfKmYL_M', 20139, N'Psittacidae: cuidados e curiosidades', N'Video adicional para enriquecer a pagina de Psittacidae.');

DECLARE @VideoId varchar(20);
DECLARE @FiltroId bigint;
DECLARE @Titulo nvarchar(300);
DECLARE @Texto nvarchar(max);
DECLARE @FiltroNome nvarchar(255);
DECLARE @StoryId bigint;
DECLARE @LivroId bigint;
DECLARE @ContentId bigint;
DECLARE @Rota nvarchar(300);
DECLARE @Html nvarchar(max);

DECLARE videos_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT v.VideoId, v.FiltroId, v.Titulo, v.Texto, f.Nome, f.StoryId, f.LivroId
    FROM @Videos v
    INNER JOIN Filtro f ON f.Id = v.FiltroId
    ORDER BY v.FiltroId, v.VideoId;

OPEN videos_cursor;
FETCH NEXT FROM videos_cursor INTO @VideoId, @FiltroId, @Titulo, @Texto, @FiltroNome, @StoryId, @LivroId;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @ContentId = NULL;
    SET @Rota = CONCAT('/natureza/video-', @FiltroId, '-', @VideoId);
    SET @Html = CONCAT(
        N'<h1>', @Titulo, N'</h1>',
        N'<p style="text-align: center;"><iframe title="', @Titulo, N'" src="https://www.youtube.com/embed/', @VideoId, N'" width="320" height="560" frameborder="0" allowfullscreen="allowfullscreen"></iframe></p>',
        N'<p>', @Texto, N'</p>',
        N'<p><strong>Filtro:</strong> ', @FiltroNome, N'</p>'
    );

    SELECT @ContentId = Id
    FROM Content
    WHERE Rotas = @Rota;

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

    FETCH NEXT FROM videos_cursor INTO @VideoId, @FiltroId, @Titulo, @Texto, @FiltroNome, @StoryId, @LivroId;
END;

CLOSE videos_cursor;
DEALLOCATE videos_cursor;

SELECT
    v.VideoId,
    v.FiltroId,
    f.Nome AS Filtro,
    f.CamadaId,
    CONCAT('/natureza/video-', v.FiltroId, '-', v.VideoId) AS Rota
FROM @Videos v
INNER JOIN Filtro f ON f.Id = v.FiltroId
ORDER BY f.CamadaId, v.FiltroId, v.VideoId;
