select * from Filtro where CriterioId is null and UltimaPasta=1

--update AspNetUsers set Compartilhar=null 
--update Content set Versiculo=68 where Id=28
--delete from HashtagContent

select * from Criterio where Id=53
select Versiculo, Id from Content where Versiculo is not null
 and Discriminator='Chave' order by Versiculo
select * from HashtagContent

select * from Story order by Capitulo

--select * from Content where Id=18690
--select * from Content where Id=50882
select * from HashtagContent where HashtagId=1

select * from HashtagFiltro
select * from Filtro order by CriterioId ASC







