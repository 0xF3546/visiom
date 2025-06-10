using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using visiom.DataAccess.Database;
using visiom.DataAccess;

namespace visiom.Api.Configuration
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly Logger<Startup> _logger;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOpenApiDocument(c =>
                c.PostProcess = document =>
                {
                    document.Info = new NSwag.OpenApiInfo
                    {
                        Title = "visiom SearchEngine",
                        Version = "v1",
                        Description = "Visiom is a modern Search Engine build with React & .NET.",
                        Contact = new NSwag.OpenApiContact
                        {
                            Name = "Visiom",
                            Email = ""
                        }
                    };
                }
            );

            services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = context =>
                        {
                            context.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        },
                        OnRedirectToAccessDenied = context =>
                        {
                            context.Response.StatusCode = 403;
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddCors(c =>
            {
                c.AddDefaultPolicy(new CorsPolicyBuilder().WithOrigins(_configuration.GetSection("CorsUrls").Get<string[]>())
                    .AllowAnyMethod()
                .AllowAnyHeader()
                    .AllowCredentials()
                    .Build());
            });

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("AppDbContext"))
            );

            services.ConfigureCoreServices();
            services.ConfigureDataAccess();
            services.ConfigureDatabase(_configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseSwaggerUi();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
