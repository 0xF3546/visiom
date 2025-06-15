using visiom.Core.Extensions;
using visiom.Crawler.dto;

namespace visiom.Core.WebCrawler.impl
{
    internal class CrawlerService : ICrawlerService
    {
        public async Task<PageResult<CrawlerResultDto>> CrawlAsync(string query, int pageSize = 10, string nextPageToken = null)
        {
            Crawler.Crawler crawler = new();
            var (results, nextToken) = await crawler.SearchWebAsync(query, pageSize, nextPageToken);
            return new PageResult<CrawlerResultDto>(results, results.Count, nextToken);
        }
    }
}
