using visiom.Core.Extensions;
using visiom.Core.Index;
using visiom.Crawler.dto;

namespace visiom.Core.WebCrawler.impl
{
    internal class CrawlerService : ICrawlerService
    {
        private readonly ISearchIndexReader _index;

        public CrawlerService(ISearchIndexReader index) => _index = index;

        public async Task<PageResult<CrawlerResultDto>> CrawlAsync(string query, IPageable pageable)
        {
            var (skip, take) = pageable.ToSlice();
            var (items, total) = await _index.SearchPagesAsync(query, skip, take);
            return new PageResult<CrawlerResultDto>(items, total);
        }

        public async Task<PageResult<ImageResultDto>> CrawlImagesAsync(string query, IPageable pageable)
        {
            var (skip, take) = pageable.ToSlice();
            var (items, total) = await _index.SearchImagesAsync(query, skip, take);
            return new PageResult<ImageResultDto>(items, total);
        }
    }
}
