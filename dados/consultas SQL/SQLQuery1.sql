
-- exibe todas as pastas que não tem versiculos
select F.Id, F.Nome, C.Versiculo from Filtro as F 
left join FiltroContent as FC on F.Id=FC.FiltroId
left join Content as C on C.Id=FC.ContentId
where C.Versiculo is null 

-- exibe todas as pastas que tem versiculos
select F.Id, F.Nome, C.Versiculo from Filtro as F 
inner join FiltroContent as FC on F.Id=FC.FiltroId
inner join Content as C on C.Id=FC.ContentId
where C.Discriminator='Chave'

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



