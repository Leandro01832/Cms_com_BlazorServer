using business.business;
using business.Group;

namespace BlazorServerCms.Data
{
    public interface IStoryService
    {
        Task<Story> GetStoryByIdAsync(long storyId);
        Task<List<Content>> GetContentsByStoryIdAsync(long storyId, int quantidadeLista,
        int quantDiv, int slideAtual, int? carregando = null);
        Task<List<FiltroContent>> GetContentsByFiltroIdAsync(long filtroId, int quantidadeLista,
        int quantDiv, int slideAtual, int? carregando = null);


        int CountPagesAsync(long storyId);
        int CountLikesAsync(long ContentId);
        bool HasFiltersAsync(long storyId);
        Task<int> GetYouTubeVideoDurationAsync(string videoId);
    }
}
