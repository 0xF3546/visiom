using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using visiom.Core.WebCrawler;
using visiom.Core.WebCrawler.impl;

namespace visiom.Core
{
    public static class Configuration
    {
        public static void ConfigureCore(this IServiceCollection services)
        {
            services.AddScoped<ICrawlerService, CrawlerService>();
        }
    }
}
