using business.business;
using business.business.Book;
using business.Group;

namespace BlazorServerCms.Data
{
    public interface IStoryService
    {
        Task<List<Content>> PaginarStory(long storyId, int quantidadeLista,
        int quantDiv, int slideAtual, Livro livro, int? carregando = null);
        Task<List<FiltroContent>> PaginarFiltro(long filtroId, int quantidadeLista,
        int quantDiv, int slideAtual, Livro livro, int? carregando = null);


       
        Task<List<Content>> GetFiltroByIdAsync(long filtroId, Livro livro, int slide = 0, int quantDiv = 0);
        int CountPagesInFilterAsync(long filtroId, Livro livro);
        int CountPagesAsync(long storyId, Livro livro);        
        bool HasFiltersAsync(long storyId , Livro livro);
        Task<int> GetYouTubeVideoDurationAsync(string videoId);
    }
}
