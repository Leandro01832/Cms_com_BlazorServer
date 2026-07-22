select * from Filtro where CriterioId is null and UltimaPasta=1

update AspNetUsers set Compartilhar=null 
--update Content set Versiculo=68 where Id=28
--delete from HashtagContent
--UPDATE Content
--SET Discriminator = 'Page'
--WHERE Discriminator = 'Comment';

select * from Anotacao

--SELECT C.Id from Content as C inner join 
--AspNetUsers as A on A.Id=C.UserModelId where C.UserModelId=''

--SELECT C.Id from Content as C inner join 
-- AspNetUsers as A on A.Id=C.UserModelId where C.UserModelId='779f69a9-aa7c-47df-bc7a-12c5934d1330'

select * from Criterio where Id=53
select Versiculo, Id from Content where Versiculo is not null
 and Discriminator='Chave' order by Versiculo
select * from AspNetUsers

select * from Story order by Capitulo

select * from FiltroContent
--select * from Content where Id=50882
select * from HashtagContent where HashtagId=1

select * from AspNetUsers
select * from Filtro where FiltroId=78

select Html from Content where Html like '%controls%'







