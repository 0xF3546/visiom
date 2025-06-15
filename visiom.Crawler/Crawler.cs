using AngleSharp;
using Newtonsoft.Json;
using visiom.Crawler.dto;

namespace visiom.Crawler
{
    public class Crawler
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, int> domainVisitCount = new Dictionary<string, int>();
        private const int MaxPagesPerDomain = 2;

        public Crawler()
        {
            _httpClient = new HttpClient();
        }

        public Crawler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(List<CrawlerResultDto> results, string nextPageToken)> SearchWebAsync(string searchInput, int pageSize = 10, string nextPageToken = null)
        {
            var seeds = GenerateSeeds(searchInput);
            var queue = nextPageToken == null ? new Queue<string>(seeds) : DeserializeQueue(nextPageToken);
            var visited = new HashSet<string>();
            var results = new List<CrawlerResultDto>();
            int maxPages = 100;

            while (queue.Count > 0 && visited.Count < maxPages && results.Count < pageSize)
            {
                var url = queue.Dequeue();
                var domain = new Uri(url).Host;
                if (visited.Contains(url) || (domainVisitCount.ContainsKey(domain) && domainVisitCount[domain] >= MaxPagesPerDomain)) continue;
                visited.Add(url);
                if (!domainVisitCount.ContainsKey(domain)) domainVisitCount[domain] = 0;
                domainVisitCount[domain]++;

                var page = await CrawlPage(url);
                if (page == null) continue;

                if (IsMatch(page, searchInput))
                {
                    var snippet = GenerateSnippet(page, searchInput);
                    results.Add(new CrawlerResultDto
                    {
                        Url = page.Url,
                        Title = page.Title,
                        Snippet = snippet,
                        CrawledAt = DateTime.Now
                    });
                }

                foreach (var link in page.Links)
                {
                    if (!visited.Contains(link))
                    {
                        queue.Enqueue(link);
                    }
                }
            }

            string nextToken = results.Count >= pageSize ? SerializeQueue(queue) : null;
            return (results.Take(pageSize).ToList(), nextToken);
        }

        private async Task<CrawledPageDto> CrawlPage(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Fehler: {url} konnte nicht geladen werden (Status: {response.StatusCode})");
                    return null;
                }
                var html = await response.Content.ReadAsStringAsync();
                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(html));
                var title = document.QuerySelector("title")?.TextContent;
                var content = document.Body?.TextContent;
                var links = document.QuerySelectorAll("a")
                    .Select(a => a.GetAttribute("href"))
                    .Where(href => !string.IsNullOrEmpty(href))
                    .Select(link => new Uri(new Uri(url), link).AbsoluteUri)
                    .ToList();

                return new CrawledPageDto
                {
                    Url = url,
                    Title = title,
                    Content = content,
                    Links = links
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Crawlen von {url}: {ex.Message}");
                return null;
            }
        }

        private List<string> GenerateSeeds(string searchInput)
        {
            return new List<string>
            {
                "https://www.wikipedia.org",
                "http://example.com",
                "https://www.bbc.com",
                "https://www.theguardian.com"
            };
        }

        private bool IsMatch(CrawledPageDto page, string searchInput)
        {
            return page.Url.Contains(searchInput, StringComparison.OrdinalIgnoreCase) ||
                   (page.Title != null && page.Title.Contains(searchInput, StringComparison.OrdinalIgnoreCase)) ||
                   (page.Content != null && page.Content.Contains(searchInput, StringComparison.OrdinalIgnoreCase));
        }

        private string GenerateSnippet(CrawledPageDto page, string searchInput)
        {
            if (page.Content != null && page.Content.Contains(searchInput, StringComparison.OrdinalIgnoreCase))
            {
                int index = page.Content.IndexOf(searchInput, StringComparison.OrdinalIgnoreCase);
                int start = Math.Max(0, index - 50);
                int end = Math.Min(page.Content.Length, index + searchInput.Length + 50);
                return "..." + page.Content.Substring(start, end - start) + "...";
            }
            else if (page.Title != null && page.Title.Contains(searchInput, StringComparison.OrdinalIgnoreCase))
            {
                return page.Title;
            }
            else
            {
                return page.Url;
            }
        }

        private string SerializeQueue(Queue<string> queue)
        {
            return JsonConvert.SerializeObject(queue.ToList());
        }

        private Queue<string> DeserializeQueue(string token)
        {
            return new Queue<string>(JsonConvert.DeserializeObject<List<string>>(token));
        }
    }
}