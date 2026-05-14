DECLARE @DataAtual datetime2 = SYSDATETIME();
DECLARE @LivroId bigint = (SELECT TOP 1 Id FROM Livro ORDER BY Id);

-- Conteudos iniciais para popular o CMS.
-- O script evita duplicidade usando a coluna Rotas.

IF EXISTS (SELECT 1 FROM Story WHERE Id = 15)
AND NOT EXISTS (SELECT 1 FROM Content WHERE Rotas = '/tecnologia/ia-no-dia-a-dia')
BEGIN
    INSERT INTO Content (Data, Titulo, StoryId, LivroId, Rotas, QuantLiked, QuantShared, Html, Discriminator, Versiculo, Posicao, UserModelId, ContentId)
    VALUES (
        @DataAtual,
        N'IA no dia a dia',
        15,
        @LivroId,
        '/tecnologia/ia-no-dia-a-dia',
        0,
        0,
        N'<h1>IA no dia a dia</h1><p>A inteligencia artificial ja aparece em pesquisas, recomendacoes, atendimento, criacao de imagens, escrita e organizacao de tarefas.</p><p>Para usar bem, comece com pedidos claros, revise as respostas e transforme a IA em apoio para pensar melhor, nao em substituta da sua decisao.</p>',
        'Pagina',
        1,
        NULL,
        NULL,
        NULL
    );
END;

IF EXISTS (SELECT 1 FROM Story WHERE Id = 15)
AND NOT EXISTS (SELECT 1 FROM Content WHERE Rotas = '/tecnologia/seguranca-digital-basica')
BEGIN
    INSERT INTO Content (Data, Titulo, StoryId, LivroId, Rotas, QuantLiked, QuantShared, Html, Discriminator, Versiculo, Posicao, UserModelId, ContentId)
    VALUES (
        @DataAtual,
        N'Seguranca digital basica',
        15,
        @LivroId,
        '/tecnologia/seguranca-digital-basica',
        0,
        0,
        N'<h1>Seguranca digital basica</h1><p>Use senhas diferentes, ative autenticacao em dois fatores e desconfie de links recebidos fora de contexto.</p><ul><li>Atualize seus aplicativos.</li><li>Evite repetir senhas importantes.</li><li>Confira o endereco do site antes de informar dados.</li></ul>',
        'Pagina',
        2,
        NULL,
        NULL,
        NULL
    );
END;

IF EXISTS (SELECT 1 FROM Story WHERE Id = 25)
AND NOT EXISTS (SELECT 1 FROM Content WHERE Rotas = '/estudos/como-organizar-uma-rotina')
BEGIN
    INSERT INTO Content (Data, Titulo, StoryId, LivroId, Rotas, QuantLiked, QuantShared, Html, Discriminator, Versiculo, Posicao, UserModelId, ContentId)
    VALUES (
        @DataAtual,
        N'Como organizar uma rotina de estudos',
        25,
        @LivroId,
        '/estudos/como-organizar-uma-rotina',
        0,
        0,
        N'<h1>Como organizar uma rotina de estudos</h1><p>Uma boa rotina combina revisao, pratica e descanso. Separe blocos curtos, defina uma meta por sessao e registre o que ficou dificil.</p><p>No fim da semana, revise os erros mais comuns. Eles mostram exatamente onde voce deve insistir.</p>',
        'Pagina',
        1,
        NULL,
        NULL,
        NULL
    );
END;

IF EXISTS (SELECT 1 FROM Story WHERE Id = 25)
AND NOT EXISTS (SELECT 1 FROM Content WHERE Rotas = '/estudos/matematica-por-problemas')
BEGIN
    INSERT INTO Content (Data, Titulo, StoryId, LivroId, Rotas, QuantLiked, QuantShared, Html, Discriminator, Versiculo, Posicao, UserModelId, ContentId)
    VALUES (
        @DataAtual,
        N'Matematica por problemas',
        25,
        @LivroId,
        '/estudos/matematica-por-problemas',
        0,
        0,
        N'<h1>Matematica por problemas</h1><p>Aprender matematica fica mais facil quando a teoria aparece junto de exemplos. Leia o enunciado, destaque os dados e escreva o que precisa descobrir.</p><p>Depois, compare sua resolucao com outro metodo. Esse contraste ajuda a fixar o raciocinio.</p>',
        'Pagina',
        2,
        NULL,
        NULL,
        NULL
    );
END;

IF EXISTS (SELECT 1 FROM Story WHERE Id = 29)
AND NOT EXISTS (SELECT 1 FROM Content WHERE Rotas = '/blog/primeiro-post')
BEGIN
    INSERT INTO Content (Data, Titulo, StoryId, LivroId, Rotas, QuantLiked, QuantShared, Html, Discriminator, Versiculo, Posicao, UserModelId, ContentId)
    VALUES (
        @DataAtual,
        N'Primeiro post do blog',
        29,
        @LivroId,
        '/blog/primeiro-post',
        0,
        0,
        N'<h1>Primeiro post do blog</h1><p>Este espaco pode reunir novidades, bastidores, tutoriais, opinioes e atualizacoes do projeto.</p><p>Uma boa publicacao tem um tema claro, um paragrafo de abertura forte e uma conclusao que convide o leitor a continuar explorando.</p>',
        'Pagina',
        1,
        NULL,
        NULL,
        NULL
    );
END;

IF EXISTS (SELECT 1 FROM Story WHERE Id = 29)
AND NOT EXISTS (SELECT 1 FROM Content WHERE Rotas = '/blog/guia-rapido-de-publicacao')
BEGIN
    INSERT INTO Content (Data, Titulo, StoryId, LivroId, Rotas, QuantLiked, QuantShared, Html, Discriminator, Versiculo, Posicao, UserModelId, ContentId)
    VALUES (
        @DataAtual,
        N'Guia rapido de publicacao',
        29,
        @LivroId,
        '/blog/guia-rapido-de-publicacao',
        0,
        0,
        N'<h1>Guia rapido de publicacao</h1><ol><li>Escolha a Story correta.</li><li>Crie um titulo direto.</li><li>Use uma rota curta e legivel.</li><li>Revise o HTML antes de salvar.</li></ol><p>Esse fluxo mantem o CMS organizado e facilita a navegacao.</p>',
        'Pagina',
        2,
        NULL,
        NULL,
        NULL
    );
END;

IF EXISTS (SELECT 1 FROM Story WHERE Id = 34)
AND NOT EXISTS (SELECT 1 FROM Content WHERE Rotas = '/empresas/apresentacao-institucional')
BEGIN
    INSERT INTO Content (Data, Titulo, StoryId, LivroId, Rotas, QuantLiked, QuantShared, Html, Discriminator, Versiculo, Posicao, UserModelId, ContentId)
    VALUES (
        @DataAtual,
        N'Apresentacao institucional',
        34,
        @LivroId,
        '/empresas/apresentacao-institucional',
        0,
        0,
        N'<h1>Apresentacao institucional</h1><p>Uma apresentacao institucional mostra quem a empresa e, o que entrega e por que o cliente pode confiar nela.</p><p>Inclua missao, diferenciais, principais servicos e canais de contato.</p>',
        'Pagina',
        1,
        NULL,
        NULL,
        NULL
    );
END;

IF EXISTS (SELECT 1 FROM Story WHERE Id = 34)
AND NOT EXISTS (SELECT 1 FROM Content WHERE Rotas = '/empresas/checklist-de-atendimento')
BEGIN
    INSERT INTO Content (Data, Titulo, StoryId, LivroId, Rotas, QuantLiked, QuantShared, Html, Discriminator, Versiculo, Posicao, UserModelId, ContentId)
    VALUES (
        @DataAtual,
        N'Checklist de atendimento',
        34,
        @LivroId,
        '/empresas/checklist-de-atendimento',
        0,
        0,
        N'<h1>Checklist de atendimento</h1><ul><li>Entenda a necessidade do cliente.</li><li>Registre o historico da conversa.</li><li>Combine o proximo passo com prazo.</li><li>Confirme se a solucao foi compreendida.</li></ul>',
        'Pagina',
        2,
        NULL,
        NULL,
        NULL
    );
END;

IF EXISTS (SELECT 1 FROM Story WHERE Id = 45)
AND NOT EXISTS (SELECT 1 FROM Content WHERE Rotas = '/info-produtos/como-descrever-produtos')
BEGIN
    INSERT INTO Content (Data, Titulo, StoryId, LivroId, Rotas, QuantLiked, QuantShared, Html, Discriminator, Versiculo, Posicao, UserModelId, ContentId)
    VALUES (
        @DataAtual,
        N'Como descrever produtos',
        45,
        @LivroId,
        '/info-produtos/como-descrever-produtos',
        0,
        0,
        N'<h1>Como descrever produtos</h1><p>Uma boa descricao responde o que e o produto, para quem ele serve, quais beneficios entrega e quais detalhes tecnicos importam.</p><p>Use frases curtas, destaque materiais, medidas, garantia e cuidados de uso.</p>',
        'Pagina',
        1,
        NULL,
        NULL,
        NULL
    );
END;

IF EXISTS (SELECT 1 FROM Story WHERE Id = 19)
AND NOT EXISTS (SELECT 1 FROM Content WHERE Rotas = '/saude/habitos-simples-de-bem-estar')
BEGIN
    INSERT INTO Content (Data, Titulo, StoryId, LivroId, Rotas, QuantLiked, QuantShared, Html, Discriminator, Versiculo, Posicao, UserModelId, ContentId)
    VALUES (
        @DataAtual,
        N'Habitos simples de bem-estar',
        19,
        @LivroId,
        '/saude/habitos-simples-de-bem-estar',
        0,
        0,
        N'<h1>Habitos simples de bem-estar</h1><p>Pequenas escolhas diarias podem melhorar energia e concentracao: beber agua, dormir melhor, caminhar e fazer pausas durante o trabalho.</p><p>Conteudo informativo nao substitui orientacao profissional de saude.</p>',
        'Pagina',
        1,
        NULL,
        NULL,
        NULL
    );
END;

IF EXISTS (SELECT 1 FROM Story WHERE Id = 2)
AND NOT EXISTS (SELECT 1 FROM Content WHERE Rotas = '/natureza/curiosidades-da-biodiversidade')
BEGIN
    INSERT INTO Content (Data, Titulo, StoryId, LivroId, Rotas, QuantLiked, QuantShared, Html, Discriminator, Versiculo, Posicao, UserModelId, ContentId)
    VALUES (
        @DataAtual,
        N'Curiosidades da biodiversidade',
        2,
        @LivroId,
        '/natureza/curiosidades-da-biodiversidade',
        0,
        0,
        N'<h1>Curiosidades da biodiversidade</h1><p>A biodiversidade reune especies, ecossistemas e relacoes que mantem o equilibrio da vida. Cada ambiente tem funcoes importantes, da polinizacao ao ciclo da agua.</p><p>Preservar habitats ajuda a proteger alimentos, clima e qualidade de vida.</p>',
        'Pagina',
        1,
        NULL,
        NULL,
        NULL
    );
END;

IF EXISTS (SELECT 1 FROM Story WHERE Id = 2)
AND NOT EXISTS (SELECT 1 FROM Content WHERE Rotas = '/natureza/agua-e-consumo-consciente')
BEGIN
    INSERT INTO Content (Data, Titulo, StoryId, LivroId, Rotas, QuantLiked, QuantShared, Html, Discriminator, Versiculo, Posicao, UserModelId, ContentId)
    VALUES (
        @DataAtual,
        N'Agua e consumo consciente',
        2,
        @LivroId,
        '/natureza/agua-e-consumo-consciente',
        0,
        0,
        N'<h1>Agua e consumo consciente</h1><p>Economizar agua envolve atitudes simples: fechar torneiras, corrigir vazamentos, reaproveitar quando possivel e escolher produtos com menor impacto ambiental.</p><p>O consumo consciente tambem depende de informacao clara e habitos consistentes.</p>',
        'Pagina',
        2,
        NULL,
        NULL,
        NULL
    );
END;
