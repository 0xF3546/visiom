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
            PageRequest pageable,
            [FromQuery(Name = "q")] string query)
        {
            var result = await _crawlerService.CrawlAsync(query, pageable);
            return Ok(result);
        }

        [HttpGet("images")]
        [ProducesResponseType(typeof(PageResult<ImageResultDto>), 200)]
        public async Task<IActionResult> SearchImages(
            PageRequest pageable,
            [FromQuery(Name = "q")] string query)
        {
            var result = await _crawlerService.CrawlImagesAsync(query, pageable);
            return Ok(result);
        }
    }
}
