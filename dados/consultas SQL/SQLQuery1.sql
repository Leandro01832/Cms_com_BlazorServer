
-- exibe todas as pastas que n�o tem versiculos
select F.Id, F.Nome, C.Versiculo from Filtro as F 
inner join FiltroContent as FC on F.Id=FC.FiltroId
inner join Content as C on C.Id=FC.ContentId
where C.Versiculo is null 

--Exibe filtros que tem mais de um vinculo com conteudo
SELECT F.Id, F.Nome, COUNT(*) AS Conteudos
FROM Filtro AS F 
inner JOIN FiltroContent AS FC ON F.Id = FC.FiltroId
WHERE  F.UltimaPasta = 0
GROUP BY F.Id, F.Nome
HAVING COUNT(*) > 0;

select * from Filtro where CamadaId>4 and UltimaPasta=0

SELECT DISTINCT f.FiltroId
FROM FiltroContent f
INNER JOIN Content c ON f.ContentId = c.Id
WHERE c.Html IS NOT NULL;



-- exibe todas as pastas que tem versiculos
select F.Id, F.Nome, F.CriterioId, C.Versiculo from Filtro as F 
inner join FiltroContent as FC on F.Id=FC.FiltroId
inner join Content as C on C.Id=FC.ContentId
where C.Discriminator='Pagina' order by F.FiltroId

select * from Content where Discriminator='Chave'

--delete FiltroContent where ContentId=4105
--delete FiltroContent where ContentId=18679

--select Id, Titulo, Versiculo, Discriminator from Content where StoryId=2 order by Id


--USE [cms] 
--GO
--INSERT INTO [dbo].[Content]
--           ([Data],[Titulo],[StoryId],[LivroId],[Rotas],[Html],[Link],
--		   [TextoLink],[Discriminator],[ContentId],[Versiculo],[UserModelId])
--     VALUES
--           ('2025-01-04','chave',2,null
--           ,null,'teste',null ,null
--           ,'Chave',null,112,null)
--GO 



