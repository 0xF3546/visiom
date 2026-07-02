using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using visiom.Core.Index;

namespace visiom.Core.Crawling
{
    internal sealed class CrawlerHostedService : BackgroundService
    {
        private readonly ISearchIndexWriter _writer;
        private readonly CrawlerOptions _options;
        private readonly ILogger<CrawlerHostedService> _logger;
        private readonly Crawler.Crawler _crawler = new();
        private readonly RobotsCache _robots;
        private readonly CrawlFrontier _frontier;
        private readonly DomainThrottle _throttle;

        private int _indexedCount;
        private int _activeFetches;

        public CrawlerHostedService(ISearchIndexWriter writer, IOptions<CrawlerOptions> options, ILogger<CrawlerHostedService> logger)
        {
            _writer = writer;
            _options = options.Value;
            _logger = logger;
            _frontier = new CrawlFrontier(_options.MaxPagesPerDomain);
            _throttle = new DomainThrottle(_options.RequestDelayMsPerDomain);

            var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            http.DefaultRequestHeaders.UserAgent.ParseAdd("VisiomCrawler/1.0 (+https://visiom.ai)");
            _robots = new RobotsCache(http);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _writer.InitializeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search index initialisation failed; crawler will not run.");
                return;
            }

            foreach (var seed in _options.Seeds ?? new List<string>())
                _frontier.Add(seed);

            if (_frontier.IsEmpty)
            {
                _logger.LogWarning("No crawler seeds configured (Crawler:Seeds); nothing to crawl.");
                return;
            }

            var workerCount = Math.Max(1, _options.Workers);
            _logger.LogInformation("Crawler starting: {Workers} workers, cap {Max} pages.", workerCount, _options.MaxIndexedPages);

            var workers = Enumerable.Range(0, workerCount)
                .Select(_ => Task.Run(() => WorkerLoop(stoppingToken), stoppingToken));

            try
            {
                await Task.WhenAll(workers);
            }
            catch (OperationCanceledException)
            {
            }

            _logger.LogInformation("Crawl loop ended. {Count} pages indexed.", Volatile.Read(ref _indexedCount));
        }

        private async Task WorkerLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && Volatile.Read(ref _indexedCount) < _options.MaxIndexedPages)
            {
                if (!_frontier.TryDequeue(out var url))
                {
                    if (Volatile.Read(ref _activeFetches) == 0) break;
                    try { await Task.Delay(250, ct); } catch { break; }
                    continue;
                }

                Interlocked.Increment(ref _activeFetches);
                try
                {
                    await ProcessAsync(url, ct);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Failed to process {Url}", url);
                }
                finally
                {
                    Interlocked.Decrement(ref _activeFetches);
                }
            }
        }

        private async Task ProcessAsync(string url, CancellationToken ct)
        {
            if (await _writer.IsIndexedAsync(url) || !await _robots.IsAllowedAsync(url))
                return;

            var domain = Crawler.Crawler.GetDomain(url) ?? url;
            var page = await _throttle.ScheduleAsync(domain, () => _crawler.FetchAsync(url, ct), ct);
            if (page == null) return;

            await _writer.IndexPageAsync(page);

            var count = Interlocked.Increment(ref _indexedCount);
            if (count % 25 == 0)
                _logger.LogInformation("Indexed {Count} pages...", count);

            foreach (var link in page.Links)
                _frontier.Add(link);
        }
    }
}
