using Microsoft.Data.Sqlite;
using visiom.Crawler.dto;

namespace visiom.Core.Index
{
    internal sealed class SqliteSearchIndexReader : ISearchIndexReader
    {
        private readonly SqliteConnectionFactory _connections;

        public SqliteSearchIndexReader(SqliteConnectionFactory connections) => _connections = connections;

        public async Task<(IReadOnlyList<CrawlerResultDto> Items, int Total)> SearchPagesAsync(string query, int skip, int take)
        {
            var match = FtsMatchQuery.Build(query);
            if (match == null) return (Array.Empty<CrawlerResultDto>(), 0);

            await using var conn = _connections.Open();
            var total = await Count(conn, "SELECT COUNT(*) FROM pages_fts WHERE pages_fts MATCH $q;", match);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT p.url, p.title, snippet(pages_fts, 1, '', '', ' … ', 15), p.crawled_at
                FROM pages_fts
                JOIN pages p ON p.id = pages_fts.rowid
                WHERE pages_fts MATCH $q
                ORDER BY bm25(pages_fts)
                LIMIT $take OFFSET $skip;";
            AddPagingParams(cmd, match, skip, take);

            var items = new List<CrawlerResultDto>();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var snippet = reader.IsDBNull(2) ? "" : reader.GetString(2);
                var title = reader.IsDBNull(1) || reader.GetString(1).Length == 0 ? reader.GetString(0) : reader.GetString(1);
                items.Add(new CrawlerResultDto
                {
                    Url = reader.GetString(0),
                    Title = title,
                    Description = snippet,
                    Snippet = snippet,
                    CrawledAt = ReadDate(reader, 3)
                });
            }

            return (items, total);
        }

        public async Task<(IReadOnlyList<ImageResultDto> Items, int Total)> SearchImagesAsync(string query, int skip, int take)
        {
            var match = FtsMatchQuery.Build(query);
            if (match == null) return (Array.Empty<ImageResultDto>(), 0);

            const string fromMatchingImages = @"
                FROM pages_fts
                JOIN pages p ON p.id = pages_fts.rowid
                JOIN images i ON i.page_id = p.id
                WHERE pages_fts MATCH $q";

            await using var conn = _connections.Open();
            var total = await Count(conn, $"SELECT COUNT(*) {fromMatchingImages};", match);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"
                SELECT i.image_url, i.alt, i.source_page_url, i.width, i.height, i.crawled_at, p.title
                {fromMatchingImages}
                ORDER BY bm25(pages_fts)
                LIMIT $take OFFSET $skip;";
            AddPagingParams(cmd, match, skip, take);

            var items = new List<ImageResultDto>();
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var alt = reader.IsDBNull(1) ? "" : reader.GetString(1);
                var pageTitle = reader.IsDBNull(6) ? "" : reader.GetString(6);
                items.Add(new ImageResultDto
                {
                    ImageUrl = reader.GetString(0),
                    ThumbnailUrl = reader.GetString(0),
                    Title = string.IsNullOrWhiteSpace(alt) ? pageTitle : alt,
                    SourcePageUrl = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Width = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    Height = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    CrawledAt = ReadDate(reader, 5)
                });
            }

            return (items, total);
        }

        private static async Task<int> Count(SqliteConnection conn, string sql, string match)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("$q", match);
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        private static void AddPagingParams(SqliteCommand cmd, string match, int skip, int take)
        {
            cmd.Parameters.AddWithValue("$q", match);
            cmd.Parameters.AddWithValue("$take", take);
            cmd.Parameters.AddWithValue("$skip", skip);
        }

        private static DateTime ReadDate(SqliteDataReader reader, int ordinal)
            => reader.IsDBNull(ordinal) ? default
                : DateTime.TryParse(reader.GetString(ordinal), out var dt) ? dt : default;
    }
}
