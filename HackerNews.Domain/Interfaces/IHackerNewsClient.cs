using HackerNews.Domain.Entities;

namespace HackerNews.Domain.Interfaces
{
    public interface IHackerNewsClient
    {
        Task<List<int>> GetBestStoryIdsAsync(CancellationToken cancellationToken);

        Task<Story> GetStoryByIdAsync(int id, CancellationToken cancellationToken);
    }
}
