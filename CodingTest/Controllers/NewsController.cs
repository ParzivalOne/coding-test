using CodingTest.Domain.Interfaces;
using CodingTest.Domain.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CodingTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController(INewsService newsService) : ControllerBase
    {
        private readonly INewsService _newsService = newsService;

        [HttpGet]
        [Route("best/{storyNumber}")]
        public async Task<IEnumerable<NewsResponse>> GetBestNStoriesAsync([FromRoute] int storyNumber)
        {
            var stories = await _newsService.GetBestNewsAsync(storyNumber);
            return stories.Select(news => new NewsResponse(news));
        }
    }
}
