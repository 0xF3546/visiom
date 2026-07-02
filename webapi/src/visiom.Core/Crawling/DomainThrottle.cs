using System.Collections.Concurrent;

namespace visiom.Core.Crawling
{
    internal sealed class DomainThrottle
    {
        private readonly int _delayMs;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _gates = new();
        private readonly ConcurrentDictionary<string, DateTime> _lastAccess = new();

        public DomainThrottle(int delayMs) => _delayMs = delayMs;

        public async Task<T> ScheduleAsync<T>(string domain, Func<Task<T>> action, CancellationToken ct)
        {
            var gate = _gates.GetOrAdd(domain, _ => new SemaphoreSlim(1, 1));
            await gate.WaitAsync(ct);
            try
            {
                if (_lastAccess.TryGetValue(domain, out var last))
                {
                    var wait = _delayMs - (int)(DateTime.UtcNow - last).TotalMilliseconds;
                    if (wait > 0) await Task.Delay(wait, ct);
                }

                return await action();
            }
            finally
            {
                _lastAccess[domain] = DateTime.UtcNow;
                gate.Release();
            }
        }
    }
}
