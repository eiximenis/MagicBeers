using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApiClients;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebClient.Data;

namespace WebClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddHttpContextAccessor();
            services.AddServerSideBlazor();
            services.AddHttpClient("dapr", c =>
            {
                var uri = Configuration.GetServiceUri("WebClient-dapr", "http") ??
                    new Uri(string.Format("{0}:{1}", Configuration["dapr:url"], Configuration["dapr:port"]));

                c.BaseAddress = uri;
            });
            services.AddTransient<IGameManagerClient>(sp =>
            {
                var uri = Configuration.GetServiceUri("gamemanagerservice", "https")?.AbsoluteUri ?? Configuration["Manager:url"];
                return new GameManagerClient(uri);
            });

            services.AddTransient<IStatisticsClient>(sp =>
            {
                var uri = Configuration.GetServiceUri("statisticsservice", "https")?.AbsoluteUri ?? Configuration["Statistics:url"];
                var logger = sp.GetRequiredService<ILogger<IStatisticsClient>>();
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("dapr");
                return new StatisticsClient(uri, client, logger);
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
            services.AddSingleton<WeatherForecastService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
