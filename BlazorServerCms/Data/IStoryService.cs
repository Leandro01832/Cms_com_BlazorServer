using business.business;
using business.business.Book;
using business.business.conteudo;

namespace BlazorServerCms.Data
{
    public interface IStoryService
    {
       
        Task<List<FiltroContent>> PaginarFiltro<T>( long filtroId, 
         int quantDiv, int slideAtual, Livro livro, int? carregando = null) where T : class;       
       
        int CountPagesInFilterAsync(long filtroId, Livro livro, Type type);                
        bool HasFiltersAsync(long storyId , Livro livro);
        Task<int> GetYouTubeVideoDurationAsync(string videoId);

        Task<List<Content>> BuscarConteudoHashtag(List<long> lista);
    }
}
