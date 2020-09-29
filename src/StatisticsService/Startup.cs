using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StatisticsService.Data;

namespace StatisticsService
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddDbContext<StatisticsDbContext>(
                options => {
                    var constr = Configuration.GetConnectionString("sqlserver");
                    var constrHelper = new SqlConnectionStringBuilder(constr);
                    if (string.IsNullOrEmpty(constrHelper.InitialCatalog))
                    {
                        constrHelper.InitialCatalog = "statisticsdb";
                    }
                    Console.WriteLine(">>>>>>>>>>>>>>>>> CONSTR IS " + constrHelper.ConnectionString);
                    options.UseSqlServer(constrHelper.ConnectionString); 
                }
            );
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCloudEvents();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<StatisticsService.Service>();
                endpoints.MapSubscribeHandler();
                endpoints.MapControllers();
            });
        }
    }
}
