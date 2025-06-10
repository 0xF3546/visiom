using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using visiom.DataAccess.Database;

namespace visiom.DataAccess
{
    public static class DatabaseConfiguration
    {
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string? appDbCintext = configuration.GetConnectionString("AppDbContext");
            if (appDbCintext == null)
            {
                throw new System.Exception("AppDbContext connection string not found");
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(appDbCintext);
            });
        }
    }
}
