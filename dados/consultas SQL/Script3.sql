WITH SequenciaCorreta AS (
    SELECT 
        id,
        -- Pega o valor do ID 99 (que � 98) e soma a posi��o na fila
       -- 67
       --  + 
        ROW_NUMBER() OVER (order by id ) as valor_sequencial
    FROM Content 
    WHERE Versiculo is not null
    -- and Discriminator = 'Chave'
)
UPDATE c
SET c.Versiculo = s.valor_sequencial
FROM Content c 
INNER JOIN SequenciaCorreta s ON c.id = s.id ;





WITH ConteudosValidos AS (
    SELECT DISTINCT
        c.id AS content_id,
        -- Min(f.CamadaId) garante que pegamos o camada para ordenação 
        -- mesmo que o conteúdo esteja associado a mais de um filtro
        MIN(f.CamadaId) AS menor_camada_id,
        MIN(f.FiltroId) AS menor_filtro_id
    FROM Filtro f
    INNER JOIN Content c ON c.id = f.CriterioId
    inner JOIN FiltroContent fc ON fc.FiltroId = f.Id 
    WHERE c.Versiculo is not null
     -- AND c.Discriminator = 'Chave'
    GROUP BY c.id -- Garante que cada Content aparece APENAS UMA VEZ na lista
),
SequenciaPerfeita AS (
    SELECT 
        content_id,
        -- Agora que a lista não tem linhas duplicadas, 
        -- o ROW_NUMBER vai gerar obrigatoriamente: 1, 2, 3, 4, 5...
      ROW_NUMBER() OVER (ORDER BY menor_filtro_id ASC) AS novo_versiculo
    FROM ConteudosValidos
)
UPDATE c
SET c.Versiculo = s.novo_versiculo
FROM Content c
INNER JOIN SequenciaPerfeita s ON c.id = s.content_id;