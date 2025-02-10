using HackerNews.Domain.Entities;

namespace HackerNews.Domain.Interfaces
{
    public interface IHackerNewsService
    {
        Task<List<Story>> GetBestStoriesAsync(int count, CancellationToken cancellationToken);
    }
}