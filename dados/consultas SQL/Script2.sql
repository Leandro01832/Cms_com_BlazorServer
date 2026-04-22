WITH ArvorePeixes AS (
    -- 1. Ponto de partida: Convertemos o Nome para VARCHAR(MAX) logo no início
    SELECT 
        Id, 
        CAST(Nome AS VARCHAR(MAX)) AS Nome, 
        CamadaId, 
        FiltroId,
        CAST(Nome AS VARCHAR(MAX)) AS Caminho,
        0 AS Nivel
    FROM FIltro 
    WHERE CamadaId = 5

    UNION ALL

    -- 2. Parte Recursiva: Usamos o alias da tabela 'p' corretamente
    SELECT 
        p.Id, 
        CAST(p.Nome AS VARCHAR(MAX)), 
        p.CamadaId, 
        p.FiltroId,
        node.Caminho + ' > ' + CAST(p.Nome AS VARCHAR(MAX)),
        node.Nivel + 1
    FROM Filtro p
    INNER JOIN ArvorePeixes node ON p.FiltroId = node.Id
)
-- 3. Resultado Final
SELECT 
    Id,
    REPLICATE('    ', Nivel) + Nome AS NomeFormatado,
    CamadaId,
    Caminho
FROM ArvorePeixes
ORDER BY Caminho;