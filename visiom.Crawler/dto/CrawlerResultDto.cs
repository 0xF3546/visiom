using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace visiom.Crawler.dto
{
    public class CrawlerResultDto
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CrawledAt { get; set; }
        public string Snippet { get; set; }
    }

}
