--select * from Filtro where CriterioId is null and UltimaPasta=1
select * from Content where Data > DATEADD(day, -1, GETDATE())
--update AspNetUsers set Decorar=10 where UserName='leandro01832' 
--update Content set Data=GETDATE() where Discriminator='UserContent' 
--delete from Hashtag where Id>1 
--UPDATE Story
--SET Capitulo = 16
--WHERE Id = 51;

select * from AspNetUsers
--select * from AspNetUsers

--SELECT C.Id from Content as C inner join 
--AspNetUsers as A on A.Id=C.UserModelId where C.UserModelId=''

--SELECT C.Id from Content as C inner join 
-- AspNetUsers as A on A.Id=C.UserModelId where C.UserModelId='779f69a9-aa7c-47df-bc7a-12c5934d1330'

--select * from Criterio where Id=53
--select Versiculo, Id from Content where Versiculo is not null
-- and Discriminator='Chave' order by Versiculo
--select * from AspNetUsers
--
--select * from Story ORDER by Capitulo
--
--select * from FiltroContent
----select * from Content where Id=50882
--select * from HashtagContent where HashtagId=1
--
--select * from AspNetUsers
--select * from Filtro where FiltroId=78
--
--select Html from Content where Html like '%controls%'




-- select * from FiltroContent



-- SELECT 
--     s.name AS SchemaName,
--     t.name AS TableName,
--     SUM(p.rows) AS TotalLinhas
-- FROM 
--     sys.tables t
-- INNER JOIN 
--     sys.schemas s ON t.schema_id = s.schema_id
-- INNER JOIN 
--     sys.partitions p ON t.object_id = p.object_id
-- WHERE 
--     p.index_id IN (0, 1) -- 0 = Heap (sem índice clusterizado), 1 = Clustered Index
-- GROUP BY 
--     s.name, 
--     t.name
-- HAVING 
--     SUM(p.rows) > 0
-- ORDER BY 
--     TotalLinhas DESC;







