using Microsoft.AspNetCore.Mvc;
using visiom.Core.Extensions;
using visiom.Core.WebCrawler;
using visiom.Crawler.dto;

namespace visiom.Api.Search
{
    [ApiController]
    [Route("/api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ICrawlerService _crawlerService;

        public SearchController(ICrawlerService crawlerService)
        {
            _crawlerService = crawlerService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PageResult<CrawlerResultDto>), 200)]
        public async Task<IActionResult> Search(
            [FromQuery(Name = "q")] string query,
            [FromQuery(Name = "pageSize")] int pageSize = 10,
            [FromQuery(Name = "nextPageToken")] string nextPageToken = null)
        {
            var result = await _crawlerService.CrawlAsync(query, pageSize, nextPageToken);
            return Ok(result);
        }
    }
}
