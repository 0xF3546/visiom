using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace visiom.Crawler.dto
{
    internal class CrawledPageDto
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Links { get; set; }
    }
}
