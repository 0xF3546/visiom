namespace visiom.Crawler.dto
{
    public class ImageResultDto
    {
        public string ImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Title { get; set; }
        public string SourcePageUrl { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public DateTime CrawledAt { get; set; }
    }
}
