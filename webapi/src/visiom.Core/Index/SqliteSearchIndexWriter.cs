using Microsoft.Data.Sqlite;
using visiom.Crawler.dto;

namespace visiom.Core.Index
{
    internal sealed class SqliteSearchIndexWriter : ISearchIndexWriter
    {
        private readonly SqliteConnectionFactory _connections;
        private readonly SemaphoreSlim _writeLock = new(1, 1);

        public SqliteSearchIndexWriter(SqliteConnectionFactory connections) => _connections = connections;

        public async Task InitializeAsync()
        {
            await using var conn = _connections.Open();
            await Execute(conn, "PRAGMA journal_mode=WAL;");
            await Execute(conn, SqliteSchema.Ddl);
        }

        public async Task<bool> IsIndexedAsync(string url)
        {
            await using var conn = _connections.Open();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT 1 FROM pages WHERE url = $u LIMIT 1;";
            cmd.Parameters.AddWithValue("$u", url);
            return await cmd.ExecuteScalarAsync() != null;
        }

        public async Task IndexPageAsync(CrawledPageDto page)
        {
            var now = DateTime.UtcNow.ToString("o");

            await _writeLock.WaitAsync();
            try
            {
                await using var conn = _connections.Open();
                await using var tx = conn.BeginTransaction();

                var pageId = await UpsertPage(conn, tx, page, now);
                foreach (var image in page.Images)
                    await InsertImage(conn, tx, pageId, page.Url, image, now);

                await tx.CommitAsync();
            }
            finally
            {
                _writeLock.Release();
            }
        }

        private static async Task<long> UpsertPage(SqliteConnection conn, SqliteTransaction tx, CrawledPageDto page, string now)
        {
            await using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"
                INSERT INTO pages(url, title, content, crawled_at)
                VALUES ($url, $title, $content, $ts)
                ON CONFLICT(url) DO NOTHING;
                SELECT id FROM pages WHERE url = $url;";
            cmd.Parameters.AddWithValue("$url", page.Url);
            cmd.Parameters.AddWithValue("$title", (object)page.Title ?? "");
            cmd.Parameters.AddWithValue("$content", (object)page.Content ?? "");
            cmd.Parameters.AddWithValue("$ts", now);
            return Convert.ToInt64(await cmd.ExecuteScalarAsync());
        }

        private static async Task InsertImage(SqliteConnection conn, SqliteTransaction tx, long pageId, string sourceUrl, PageImage image, string now)
        {
            await using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"
                INSERT INTO images(page_id, image_url, alt, source_page_url, width, height, crawled_at)
                VALUES ($pid, $iurl, $alt, $src, $w, $h, $ts)
                ON CONFLICT(image_url) DO NOTHING;";
            cmd.Parameters.AddWithValue("$pid", pageId);
            cmd.Parameters.AddWithValue("$iurl", image.Url);
            cmd.Parameters.AddWithValue("$alt", (object)image.Alt ?? "");
            cmd.Parameters.AddWithValue("$src", sourceUrl);
            cmd.Parameters.AddWithValue("$w", (object)image.Width ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$h", (object)image.Height ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$ts", now);
            await cmd.ExecuteNonQueryAsync();
        }

        private static async Task Execute(SqliteConnection conn, string sql)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
