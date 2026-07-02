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
        public async Task<BestNNewsResponse> GetBestNStoriesAsync([FromRoute] int storyNumber)
        {
            return await _newsService.GetBestNewsAsync(storyNumber);
        }
    }
}
