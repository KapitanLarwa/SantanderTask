using System.Text.Json;
using HackerNews.Domain.Entities;
using HackerNews.Domain.Interfaces;

namespace HackerNews.BusinessLogic.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly IHackerNewsClient _client;
        private readonly IRedisCacheService _cacheService;
        private const string BestStoriesCacheKey = "best_stories";
        private readonly int cacheTime = 5; 

        public HackerNewsService(IHackerNewsClient client, IRedisCacheService cacheService)
        {
            _client = client;
            _cacheService = cacheService;
        }

        public async Task<List<Story>> GetBestStoriesAsync(int count, CancellationToken cancellationToken)
        {
            var cachedStories = await _cacheService.GetAsync<List<Story>>(BestStoriesCacheKey);
            if (cachedStories != null && cachedStories.Count >= count)
            {
                return cachedStories.Take(count).ToList();
            }

            var storyIds = await _client.GetBestStoryIdsAsync(cancellationToken);

            var tasks = storyIds.Take(count)
                .Select(id => FetchStoryAsync(id, cancellationToken))
                .ToList();

            var stories = (await Task.WhenAll(tasks))
                .Where(s => s != null)
                .OrderByDescending(s => s.Score)
                .ToList();

            await _cacheService.SetAsync(BestStoriesCacheKey, stories, TimeSpan.FromMinutes(cacheTime));

            return stories;
        }

        private async Task<Story> FetchStoryAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var story = await _client.GetStoryByIdAsync(id, cancellationToken);
                return story;
            }
            catch (HttpRequestException)
            {
                return new Story();
            }
        }
    }
}