WITH SequenciaCorreta AS (
    SELECT 
        id,
        -- Pega o valor do ID 99 (que é 98) e soma a posição na fila
        (SELECT Versiculo FROM Content WHERE id = 99) + 
        ROW_NUMBER() OVER (ORDER BY id) as valor_sequencial
    FROM Content
    WHERE id >= 100
)
UPDATE c
SET c.Versiculo = s.valor_sequencial
FROM Content c
INNER JOIN SequenciaCorreta s ON c.id = s.id;