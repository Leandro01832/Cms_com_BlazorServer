using business.business;
using business.Group;

namespace BlazorServerCms.Data
{
    public interface IStoryService
    {
        Task<List<Content>> PaginarStory(long storyId, int quantidadeLista,
        int quantDiv, int slideAtual, int? carregando = null);
        Task<List<FiltroContent>> PaginarFiltro(long filtroId, int quantidadeLista,
        int quantDiv, int slideAtual, int? carregando = null);


        Task<Story> GetStoryByIdAsync(long storyId);
        Task<List<FiltroContent>> GetFiltroByIdAsync(long filtroId);
        int CountPagesInFilterAsync(long filtroId);
        int CountPagesAsync(long storyId);
        int CountLikesAsync(long ContentId);
        bool HasFiltersAsync(long storyId);
        Task<int> GetYouTubeVideoDurationAsync(string videoId);
    }
}
