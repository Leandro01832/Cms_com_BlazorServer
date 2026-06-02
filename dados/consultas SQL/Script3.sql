WITH SequenciaCorreta AS (
    SELECT 
        id,
        -- Pega o valor do ID 99 (que � 98) e soma a posi��o na fila
       -- 67
       --  + 
        ROW_NUMBER() OVER (order by id ) as valor_sequencial
    FROM Content 
    WHERE Versiculo is not null and Discriminator = 'Chave'
)
UPDATE c
SET c.Versiculo = s.valor_sequencial
FROM Content c 
INNER JOIN SequenciaCorreta s ON c.id = s.id ;