using AngleSharp;
using Newtonsoft.Json;
using visiom.Crawler.dto;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace visiom.Crawler
{
    public class Crawler
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, int> _domainVisitCount = new();
        private const int MaxPagesPerDomain = 2;
        private const int MaxTotalPages = 100;

        public Crawler() : this(new HttpClient(
            new HttpClientHandler
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            }))
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("VisiomCrawler/1.0 (+https://visiom.ai)");
        }

        public Crawler(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(List<CrawlerResultDto> Results, string NextPageToken)> SearchWebAsync(string searchInput, int pageSize = 10, string nextPageToken = null)
        {
            var seeds = GenerateSeeds(searchInput);
            var queue = nextPageToken == null ? new Queue<string>(seeds) : DeserializeQueue(nextPageToken);
            var visited = new HashSet<string>();
            var results = new List<CrawlerResultDto>();

            while (queue.Count > 0 && visited.Count < MaxTotalPages && results.Count < pageSize)
            {
                var url = NormalizeUrl(queue.Dequeue());
                if (string.IsNullOrWhiteSpace(url)) continue;

                var domain = GetDomain(url);
                if (visited.Contains(url) || (_domainVisitCount.TryGetValue(domain, out int count) && count >= MaxPagesPerDomain))
                    continue;

                visited.Add(url);
                _domainVisitCount[domain] = _domainVisitCount.GetValueOrDefault(domain) + 1;

                var page = await CrawlPageAsync(url);
                if (page == null) continue;

                if (IsMatch(page, searchInput))
                {
                    results.Add(new CrawlerResultDto
                    {
                        Url = page.Url,
                        Title = page.Title,
                        Snippet = GenerateSnippet(page, searchInput),
                        CrawledAt = DateTime.UtcNow
                    });
                }

                foreach (var link in page.Links)
                {
                    var cleanLink = NormalizeUrl(link);
                    if (!string.IsNullOrEmpty(cleanLink) && !visited.Contains(cleanLink))
                    {
                        queue.Enqueue(cleanLink);
                    }
                }
            }

            var nextToken = results.Count >= pageSize ? SerializeQueue(queue) : null;
            return (results.Take(pageSize).ToList(), nextToken);
        }

        private async Task<CrawledPageDto> CrawlPageAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                var html = await response.Content.ReadAsStringAsync();
                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(html));

                var title = document.QuerySelector("title")?.TextContent?.Trim();
                var content = document.Body?.TextContent ?? "";
                var links = document.QuerySelectorAll("a")
                    .Select(a => a.GetAttribute("href"))
                    .Where(href => !string.IsNullOrWhiteSpace(href) && Uri.IsWellFormedUriString(href, UriKind.RelativeOrAbsolute))
                    .Select(link => new Uri(new Uri(url), link).AbsoluteUri)
                    .Distinct()
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
                Console.WriteLine($"[ERROR] Failed to crawl {url}: {ex.Message}");
                return null;
            }
        }

        private List<string> GenerateSeeds(string searchInput)
        {
            // Just for test cases
            return new List<string>
            {
                "https://www.wikipedia.org",
                "https://www.bbc.com",
                "https://www.theguardian.com",
                "https://www.stackoverflow.com"
            };
        }

        private bool IsMatch(CrawledPageDto page, string searchInput)
        {
            return page.Url.Contains(searchInput, StringComparison.OrdinalIgnoreCase) ||
                   (!string.IsNullOrEmpty(page.Title) && page.Title.Contains(searchInput, StringComparison.OrdinalIgnoreCase)) ||
                   (!string.IsNullOrEmpty(page.Content) && page.Content.Contains(searchInput, StringComparison.OrdinalIgnoreCase));
        }

        private string GenerateSnippet(CrawledPageDto page, string searchInput)
        {
            if (!string.IsNullOrEmpty(page.Content) && page.Content.Contains(searchInput, StringComparison.OrdinalIgnoreCase))
            {
                int index = page.Content.IndexOf(searchInput, StringComparison.OrdinalIgnoreCase);
                int start = Math.Max(0, index - 50);
                int end = Math.Min(page.Content.Length, index + searchInput.Length + 50);
                return "..." + page.Content.Substring(start, end - start).Replace("\n", " ").Replace("\r", "") + "...";
            }

            return !string.IsNullOrEmpty(page.Title) ? page.Title : page.Url;
        }

        private string SerializeQueue(Queue<string> queue)
        {
            return JsonConvert.SerializeObject(queue.ToList());
        }

        private Queue<string> DeserializeQueue(string token)
        {
            var list = JsonConvert.DeserializeObject<List<string>>(token);
            return new Queue<string>(list ?? new List<string>());
        }

        private string GetDomain(string url)
        {
            return new Uri(url).Host.ToLowerInvariant();
        }

        private string NormalizeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            try
            {
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri)
                    return null;

                return uri.GetLeftPart(UriPartial.Path).TrimEnd('/');
            }
            catch
            {
                return null;
            }
        }
    }
}
