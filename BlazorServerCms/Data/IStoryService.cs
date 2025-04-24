using business.business;
using business.Group;

namespace BlazorServerCms.Data
{
    public interface IStoryService
    {
        Task<Story?> GetStoryByIdAsync(long storyId);
        Task<List<Content>> GetContentsByStoryIdAsync(long storyId);
        Task<int> CountPagesAsync(string connectionString);
        Task<int> CountLikesAsync(string connectionString);
        Task<bool> HasFiltersAsync(string connectionString);
        Task<int> GetYouTubeVideoDurationAsync(string videoId);
    }
}
