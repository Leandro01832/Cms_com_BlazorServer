using business.business;
using business.Group;

namespace BlazorServerCms.Data
{
    public class StoryService : IStoryService
    {
        public Task<int> CountLikesAsync(string connectionString)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountPagesAsync(string connectionString)
        {
            throw new NotImplementedException();
        }

        public Task<List<Content>> GetContentsByStoryIdAsync(long storyId)
        {
            throw new NotImplementedException();
        }

        public Task<Story?> GetStoryByIdAsync(long storyId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetYouTubeVideoDurationAsync(string videoId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasFiltersAsync(string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}
