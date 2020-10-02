using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ApiClients;
using Google.Protobuf.WellKnownTypes;
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
            if (Configuration.UsingDapr())
            {
                services.AddHttpClient("dapr", c =>
                {
                    var uri = Configuration.GetServiceUri("WebClient-dapr", "http");
                    if (uri == null)
                    {
                        if (int.TryParse(Configuration["DAPR_HTTP_PORT"], out var daprk8sPort))
                        {
                            uri = new Uri($"http://localhost:{daprk8sPort}");
                        }
                        else
                        {
                            uri = new Uri(string.Format("{0}:{1}", Configuration["dapr:url"], Configuration["dapr:port"]));
                        }
                    }
                    c.BaseAddress = uri;
                });
            }

            var tyeBinding = !string.IsNullOrEmpty(Configuration["Tye:Binding"]) ? Configuration["Tye:Binding"] : "https";
            if (tyeBinding == "default") tyeBinding = null;
            services.AddTransient<IGameManagerClient>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<IGameManagerClient>>();
                var uri = Configuration.GetServiceUri("gamemanagerservice", tyeBinding)?.AbsoluteUri ?? Configuration["Manager:url"];
                return new GameManagerClient(uri, logger);
            });


            services.AddTransient<IStatisticsClient>(sp =>
            {
                var uri = Configuration.GetServiceUri("statisticsservice", tyeBinding)?.AbsoluteUri ?? Configuration["Statistics:url"];
                var logger = sp.GetRequiredService<ILogger<IStatisticsClient>>();
                var client = Configuration.UsingDapr() ? sp.GetRequiredService<IHttpClientFactory>().CreateClient("dapr") : null;
                return new StatisticsClient(uri, client, logger);
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
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
                if (!Configuration.IsRunningOnKubernetes()) {
                    app.UseHsts();
                }
            }

            if (!Configuration.IsRunningOnKubernetes())
            {
                app.UseHttpsRedirection();
            }
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

    static class ConfigurationExtensions
    {
        internal static bool UsingDapr(this IConfiguration cfg)
        {
            var useDapr = true;
            if (bool.TryParse(cfg["Dapr:Enabled"], out var enabled))
            {
                useDapr = enabled;
            }

            return useDapr;
        }
        internal static bool IsRunningOnKubernetes(this IConfiguration cfg) => !string.IsNullOrEmpty(cfg["KUBERNETES_SERVICE_HOST"]);
    }
}
