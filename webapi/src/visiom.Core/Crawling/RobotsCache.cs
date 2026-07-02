using System.Collections.Concurrent;

namespace visiom.Core.Crawling
{
    // Caches per-host robots.txt disallow rules for the groups that apply to us
    // (User-agent: * or VisiomCrawler). Fails open on any error or missing file.
    internal sealed class RobotsCache
    {
        private const string UserAgent = "visiomcrawler";
        private readonly HttpClient _http;
        private readonly ConcurrentDictionary<string, Task<List<string>>> _rulesByHost = new();

        public RobotsCache(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> IsAllowedAsync(string url)
        {
            try
            {
                var uri = new Uri(url);
                var disallow = await _rulesByHost.GetOrAdd(uri.Host, h => FetchRulesAsync(uri));
                if (disallow.Count == 0) return true;

                var path = uri.AbsolutePath;
                return !disallow.Any(prefix => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return true;
            }
        }

        private async Task<List<string>> FetchRulesAsync(Uri uri)
        {
            var disallow = new List<string>();
            try
            {
                var robotsUrl = $"{uri.Scheme}://{uri.Host}/robots.txt";
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var response = await _http.GetAsync(robotsUrl, cts.Token);
                if (!response.IsSuccessStatusCode)
                    return disallow;

                var text = await response.Content.ReadAsStringAsync(cts.Token);

                bool applies = false;
                foreach (var raw in text.Split('\n'))
                {
                    var line = raw.Trim();
                    var hash = line.IndexOf('#');
                    if (hash >= 0) line = line[..hash].Trim();
                    if (line.Length == 0) continue;

                    var sep = line.IndexOf(':');
                    if (sep < 0) continue;
                    var field = line[..sep].Trim().ToLowerInvariant();
                    var value = line[(sep + 1)..].Trim();

                    if (field == "user-agent")
                        applies = value == "*" || value.ToLowerInvariant().Contains(UserAgent);
                    else if (field == "disallow" && applies && value.Length > 0)
                        disallow.Add(value);
                }
            }
            catch
            {
            }
            return disallow;
        }
    }
}
