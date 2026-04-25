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