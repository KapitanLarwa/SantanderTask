using HackerNews.Domain.Entities;
using HackerNews.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BestStoriesController : ControllerBase
    {
        private readonly IHackerNewsService _hackerNewsService;

        public BestStoriesController(IHackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;
        }

        [HttpGet("{count}")]
        public async Task<IActionResult> GetBestStories(int count, CancellationToken cancellationToken)
        {
            if (count <= 0)
                return BadRequest("Count must be greater than zero.");

            var stories = await _hackerNewsService.GetBestStoriesAsync(count, cancellationToken);
            var response = stories.Select(story => new StoryDto(story)).ToList();
            return Ok(response);
        }
    }
}