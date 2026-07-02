using System.Collections.Generic;

namespace visiom.Crawler.dto
{
    public class CrawledPageDto
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Links { get; set; } = new();
        public List<PageImage> Images { get; set; } = new();
    }

    public class PageImage
    {
        public string Url { get; set; }
        public string Alt { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
}
