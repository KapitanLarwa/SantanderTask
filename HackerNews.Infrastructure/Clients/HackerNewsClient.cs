using HackerNews.Domain.Entities;
using HackerNews.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace HackerNews.Infrastructure.Clients
{
    public class HackerNewsClient : IHackerNewsClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _bestStoriesUrl;
        private readonly string _storyUrlTemplate;


        public HackerNewsClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _bestStoriesUrl = configuration["HackerNews:BestStoriesUrl"];
            _storyUrlTemplate = configuration["HackerNews:StoryUrl"];
        }

        public async Task<List<int>> GetBestStoryIdsAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetStringAsync(_bestStoriesUrl, cancellationToken);
            return JsonSerializer.Deserialize<List<int>>(response);
        }

        public async Task<Story> GetStoryByIdAsync(int id, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetStringAsync(string.Format(_storyUrlTemplate, id), cancellationToken);
            var test = JsonSerializer.Deserialize<Story>(response);
            return test;
        }
    }
}
