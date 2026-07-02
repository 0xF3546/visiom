using Microsoft.Data.Sqlite;
using visiom.Core.Crawling;

namespace visiom.Core.Index
{
    internal sealed class SqliteConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteConnectionFactory(CrawlerOptions options)
        {
            var path = string.IsNullOrWhiteSpace(options.IndexPath) ? "visiom-index.db" : options.IndexPath;
            _connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = path,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Shared
            }.ToString();
        }

        public SqliteConnection Open()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
