using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using visiom.Core.Crawling;
using visiom.Core.Index;
using visiom.Core.WebCrawler;
using visiom.Core.WebCrawler.impl;

namespace visiom.Core
{
    public static class Configuration
    {
        public static void ConfigureCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CrawlerOptions>(configuration.GetSection(CrawlerOptions.SectionName));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<CrawlerOptions>>().Value);

            services.AddSingleton<SqliteConnectionFactory>();
            services.AddSingleton<ISearchIndexWriter, SqliteSearchIndexWriter>();
            services.AddSingleton<ISearchIndexReader, SqliteSearchIndexReader>();

            services.AddScoped<ICrawlerService, CrawlerService>();
            services.AddHostedService<CrawlerHostedService>();
        }
    }
}
