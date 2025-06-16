using visiom.Core.Extensions;
using visiom.Crawler.dto;

namespace visiom.Core.WebCrawler
{
    public interface ICrawlerService
    {
        public Task<PageResult<CrawlerResultDto>> CrawlAsync(string query, IPageable pageable, string nextPageToken = null);
    }
}
