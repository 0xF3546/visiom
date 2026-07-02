using visiom.Crawler.dto;

namespace visiom.Core.Index
{
    public interface ISearchIndexReader
    {
        Task<(IReadOnlyList<CrawlerResultDto> Items, int Total)> SearchPagesAsync(string query, int skip, int take);

        Task<(IReadOnlyList<ImageResultDto> Items, int Total)> SearchImagesAsync(string query, int skip, int take);
    }
}
