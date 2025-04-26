using business.business;
using business.Group;

namespace BlazorServerCms.Data
{
    public interface IStoryService
    {
        Task<Story> GetStoryByIdAsync(long storyId);
        Task<List<Content>> GetContentsByStoryIdAsync(long storyId, int quantidadeLista, int quantDiv, int slideAtual);
        int CountPagesAsync(long storyId);
        int CountLikesAsync(long ContentId);
        bool HasFiltersAsync(long storyId);
        Task<int> GetYouTubeVideoDurationAsync(string videoId);
    }
}
