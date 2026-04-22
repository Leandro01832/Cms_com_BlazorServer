WITH ArvorePorHashtag AS (
    -- 1. ÂNCORA: Grupos que possuem a hashtag específica
    SELECT 
        g.Id, 
        g.FiltroId, 
        g.Nome,
        g.CamadaId
    FROM Filtro g
    INNER JOIN FiltroContent gh ON g.Id = gh.FiltroId
    INNER JOIN HashtagContent h ON gh.ContentId = h.ContentId
    INNER JOIN Hashtag ha ON h.HashtagId = ha.Id
    WHERE ha.Name like '%baleia azul%'  -- A hashtag que o usuário escolheu

    UNION ALL

    -- 2. RECURSIVIDADE: Busca todos os descendentes desses grupos
    SELECT 
        t.Id, 
        t.FiltroId, 
        t.Nome,
        t.CamadaId
    FROM Filtro t
    INNER JOIN ArvorePorHashtag r ON t.FiltroId = r.Id
)
-- 3. RESULTADO: Todos os IDs relacionados àquela hashtag e seus subgrupos
SELECT DISTINCT Id, Nome, CamadaId FROM ArvorePorHashtag where Nome like '%ma%';