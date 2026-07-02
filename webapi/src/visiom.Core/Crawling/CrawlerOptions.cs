namespace visiom.Core.Crawling
{
    public class CrawlerOptions
    {
        public const string SectionName = "Crawler";

        public List<string> Seeds { get; set; } = new();

        public string IndexPath { get; set; } = "visiom-index.db";

        public int MaxIndexedPages { get; set; } = 5000;

        public int MaxPagesPerDomain { get; set; } = 200;

        public int RequestDelayMsPerDomain { get; set; } = 1000;

        public int Workers { get; set; } = 4;
    }
}
