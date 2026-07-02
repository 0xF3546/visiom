using visiom.Crawler.dto;

namespace visiom.Core.Index
{
    public interface ISearchIndexWriter
    {
        Task InitializeAsync();

        Task<bool> IsIndexedAsync(string url);

        Task IndexPageAsync(CrawledPageDto page);
    }
}
