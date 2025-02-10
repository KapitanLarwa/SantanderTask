using HackerNews.BusinessLogic.Services;
using HackerNews.Domain.Entities;
using HackerNews.Domain.Interfaces;
using Moq;

namespace HackerNews.Tests
{
    public class HackerNewsServiceTests
    {
        private readonly Mock<IHackerNewsClient> _mockClient;
        private readonly Mock<IRedisCacheService> _mockCacheService;
        private readonly IHackerNewsService _service;

        public HackerNewsServiceTests()
        {
            _mockClient = new Mock<IHackerNewsClient>();
            _mockCacheService = new Mock<IRedisCacheService>();
            _service = new HackerNewsService(_mockClient.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetBestStoriesAsync_ReturnsStoriesFromCache()
        {
            // Arrange
            var cachedStories = new List<Story>
            {
                new Story { Title = "Test Story 1", UnixTime = 1672531200, Score = 200, Kids = new List<int> {1, 2, 3} },
                new Story { Title = "Test Story 2", UnixTime = 1672531300, Score = 100, Kids = new List<int> {4, 5} }
            };
            _mockCacheService.Setup(x => x.GetAsync<List<Story>>(It.IsAny<string>())).ReturnsAsync(cachedStories.OrderByDescending(s => s.Score).ToList());

            // Act
            var result = await _service.GetBestStoriesAsync(2, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Story 1", result[0].Title);
        }

        [Fact]
        public async Task GetBestStoriesAsync_FetchesFromClient_WhenCacheIsEmpty()
        {
            // Arrange
            _mockCacheService.Setup(x => x.GetAsync<List<Story>>(It.IsAny<string>())).ReturnsAsync((List<Story>)null);
            _mockClient.Setup(x => x.GetBestStoryIdsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<int> { 101, 102 });
            _mockClient.Setup(x => x.GetStoryByIdAsync(101, It.IsAny<CancellationToken>())).ReturnsAsync(new Story { Title = "Test Story 1", UnixTime = 1672531200, Score = 200, Kids = new List<int> { 1, 2, 3 } });
            _mockClient.Setup(x => x.GetStoryByIdAsync(102, It.IsAny<CancellationToken>())).ReturnsAsync(new Story { Title = "Test Story 2", UnixTime = 1672531300, Score = 100, Kids = new List<int> { 4, 5 } });

            // Act
            var result = await _service.GetBestStoriesAsync(2, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Story 1", result[0].Title);
            _mockCacheService.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<List<Story>>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task GetBestStoriesAsync_ReturnsEmptyList_WhenNoStoriesAvailable()
        {
            // Arrange
            _mockCacheService.Setup(x => x.GetAsync<List<Story>>(It.IsAny<string>())).ReturnsAsync((List<Story>)null);
            _mockClient.Setup(x => x.GetBestStoryIdsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<int>());

            // Act
            var result = await _service.GetBestStoriesAsync(5, CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetBestStoriesAsync_HandlesStoryFetchFailures()
        {
            // Arrange
            _mockCacheService.Setup(x => x.GetAsync<List<Story>>(It.IsAny<string>())).ReturnsAsync((List<Story>)null);
            _mockClient.Setup(x => x.GetBestStoryIdsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<int> { 201, 202 });
            _mockClient.Setup(x => x.GetStoryByIdAsync(201, It.IsAny<CancellationToken>())).ReturnsAsync((Story)null);
            _mockClient.Setup(x => x.GetStoryByIdAsync(202, It.IsAny<CancellationToken>())).ReturnsAsync(new Story { Title = "Valid Story", UnixTime = 1672531400, Score = 150, Kids = new List<int> { 6, 7 } });

            // Act
            var result = await _service.GetBestStoriesAsync(2, CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal("Valid Story", result[0].Title);
        }

        [Fact]
        public async Task GetBestStoriesAsync_CachesFetchedStories()
        {
            // Arrange
            _mockCacheService.Setup(x => x.GetAsync<List<Story>>(It.IsAny<string>())).ReturnsAsync((List<Story>)null);
            _mockClient.Setup(x => x.GetBestStoryIdsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<int> { 301 });
            var story = new Story { Title = "Cached Story", UnixTime = 1672531500, Score = 250, Kids = new List<int> { 8, 9, 10 } };
            _mockClient.Setup(x => x.GetStoryByIdAsync(301, It.IsAny<CancellationToken>())).ReturnsAsync(story);

            // Act
            var result = await _service.GetBestStoriesAsync(1, CancellationToken.None);

            // Assert
            Assert.Single(result);
            Assert.Equal("Cached Story", result[0].Title);
            _mockCacheService.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<List<Story>>(), It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}
