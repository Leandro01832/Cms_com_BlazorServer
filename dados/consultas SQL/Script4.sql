WITH ArvorePorHashtag AS (
    -- 1. �NCORA: Grupos que possuem a hashtag espec�fica
    SELECT g.Id, g.FiltroId, g.Nome, g.CamadaId FROM Filtro g
    INNER JOIN HashtagFiltro gh ON g.Id = gh.FiltroId
    INNER JOIN Hashtag ha ON gh.HashtagId = ha.Id
    WHERE ha.Name like '%peixes%'  -- A hashtag que o usu�rio escolheu

    UNION ALL

    -- 2. RECURSIVIDADE: Busca todos os descendentes desses grupos
    SELECT t.Id, t.FiltroId, t.Nome, t.CamadaId FROM Filtro t
    INNER JOIN ArvorePorHashtag r ON t.FiltroId = r.Id
)
-- 3. RESULTADO: Todos os IDs relacionados �quela hashtag e seus subgrupos
SELECT DISTINCT Id, Nome, CamadaId FROM ArvorePorHashtag ;


-- Consulta para extrair os links das tags <img> e identificar
-- quais são compartilhados entre diferentes conteúdos
WITH LinksProcessados AS (
    SELECT 
        Id, 
        Titulo, StoryId,
        -- Extrai o texto entre src=" e a próxima aspas
        SUBSTRING(
            Html, 
            CHARINDEX('src="', Html) + 5, 
            CHARINDEX('"', Html, CHARINDEX('src="', Html) + 5) - (CHARINDEX('src="', Html) + 5)
        ) AS Link
    FROM Content
    WHERE Html LIKE '%src="%' 
)
SELECT a.Id, a.Titulo, a.StoryId, a.Link
FROM LinksProcessados a
WHERE EXISTS (
    SELECT 1 
    FROM LinksProcessados b 
    WHERE a.Link = b.Link AND a.Id <> b.Id 
)
ORDER BY a.Link;