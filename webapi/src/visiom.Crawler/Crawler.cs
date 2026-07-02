using AngleSharp;
using visiom.Crawler.dto;

namespace visiom.Crawler
{
    // Pure page fetcher: downloads a URL and extracts title, text, links and images.
    // Holds no crawl state; the frontier/politeness/indexing live in the crawl service.
    public class Crawler
    {
        private readonly HttpClient _httpClient;

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

        public async Task<CrawledPageDto> FetchAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return null;

                var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
                if (!contentType.Contains("html", StringComparison.OrdinalIgnoreCase))
                    return null;

                var html = await response.Content.ReadAsStringAsync(cancellationToken);
                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(html), cancellationToken);

                var pageUri = new Uri(url);
                var title = document.QuerySelector("title")?.TextContent?.Trim();
                var content = CollapseWhitespace(document.Body?.TextContent ?? "");

                var links = document.QuerySelectorAll("a")
                    .Select(a => a.GetAttribute("href"))
                    .Where(href => !string.IsNullOrWhiteSpace(href) && Uri.IsWellFormedUriString(href, UriKind.RelativeOrAbsolute))
                    .Select(link => TryAbsolute(pageUri, link))
                    .Where(link => link != null)
                    .Distinct()
                    .ToList();

                var images = document.QuerySelectorAll("img")
                    .Select(img => BuildImage(pageUri, img))
                    .Where(img => img != null)
                    .ToList();

                return new CrawledPageDto
                {
                    Url = url,
                    Title = title,
                    Content = content,
                    Links = links,
                    Images = images
                };
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRAWL] Failed to fetch {url}: {ex.Message}");
                return null;
            }
        }

        private static string CollapseWhitespace(string text)
            => System.Text.RegularExpressions.Regex.Replace(text, "\\s+", " ").Trim();

        private PageImage BuildImage(Uri pageUri, AngleSharp.Dom.IElement img)
        {
            var src = img.GetAttribute("src");
            if (string.IsNullOrWhiteSpace(src))
                src = img.GetAttribute("data-src");
            if (string.IsNullOrWhiteSpace(src) || src.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                return null;

            var abs = TryAbsolute(pageUri, src);
            if (abs == null) return null;

            var lower = abs.ToLowerInvariant();
            if (lower.EndsWith(".svg") || lower.Contains("sprite") || lower.Contains("/icons/") || lower.Contains("logo"))
                return null;

            int? width = ParseDimension(img.GetAttribute("width"));
            int? height = ParseDimension(img.GetAttribute("height"));

            if ((width.HasValue && width < 100) || (height.HasValue && height < 100))
                return null;

            return new PageImage
            {
                Url = abs,
                Alt = img.GetAttribute("alt") ?? img.GetAttribute("title"),
                Width = width,
                Height = height
            };
        }

        private static int? ParseDimension(string value)
            => int.TryParse(value, out var n) ? n : (int?)null;

        private static string TryAbsolute(Uri baseUri, string href)
        {
            try
            {
                return new Uri(baseUri, href).AbsoluteUri;
            }
            catch
            {
                return null;
            }
        }

        // Canonicalises for dedup: http/https only, query kept, fragment and trailing
        // slash dropped. Returns null for anything that isn't crawlable.
        public static string NormalizeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            try
            {
                var uri = new Uri(url, UriKind.RelativeOrAbsolute);
                if (!uri.IsAbsoluteUri)
                    return null;

                if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                    return null;

                return uri.GetLeftPart(UriPartial.Query).TrimEnd('/');
            }
            catch
            {
                return null;
            }
        }

        public static string GetDomain(string url)
        {
            try
            {
                return new Uri(url).Host.ToLowerInvariant();
            }
            catch
            {
                return null;
            }
        }
    }
}
