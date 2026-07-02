using System.Collections.Concurrent;

namespace visiom.Core.Crawling
{
    internal sealed class CrawlFrontier
    {
        private const int MaxSize = 100_000;

        private readonly int _maxPagesPerDomain;
        private readonly ConcurrentQueue<string> _queue = new();
        private readonly ConcurrentDictionary<string, byte> _seen = new();
        private readonly ConcurrentDictionary<string, int> _domainCount = new();

        public CrawlFrontier(int maxPagesPerDomain) => _maxPagesPerDomain = maxPagesPerDomain;

        public bool IsEmpty => _queue.IsEmpty;

        public bool TryDequeue(out string url) => _queue.TryDequeue(out url);

        public void Add(string url)
        {
            if (_queue.Count >= MaxSize) return;

            var normalized = Crawler.Crawler.NormalizeUrl(url);
            if (normalized == null) return;

            var domain = Crawler.Crawler.GetDomain(normalized);
            if (domain == null) return;

            if (_domainCount.TryGetValue(domain, out var count) && count >= _maxPagesPerDomain) return;
            if (!_seen.TryAdd(normalized, 0)) return;

            _domainCount.AddOrUpdate(domain, 1, (_, c) => c + 1);
            _queue.Enqueue(normalized);
        }
    }
}
