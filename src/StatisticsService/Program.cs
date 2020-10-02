using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StatisticsService.Extensions;

namespace StatisticsService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var startupConf = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.{Environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

            CreateHostBuilder(args, startupConf).
                Build().CreateDbIfNeeded()
                .Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration startupConf) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {

                        var (httpPort, httpsPort) = GetPorts(startupConf);

                        Console.WriteLine($">>>>> HTTP PORT: {httpPort}, HTTPS PORT: {httpsPort}");

                        if (httpPort != -1)
                        {
                            options.ListenAnyIP(httpPort, lo =>
                            {
                                lo.Protocols = httpsPort != -1 ? HttpProtocols.Http1AndHttp2 : HttpProtocols.Http2;
                            });
                        }
                        if (httpsPort != -1)
                        {
                            options.ListenAnyIP(httpsPort, lo =>
                            {
                                lo.UseHttps();
                                lo.Protocols = HttpProtocols.Http2;
                            });
                        }
                    });
                    webBuilder.UseStartup<Startup>();
                });


        private static (int http, int https) GetPorts(IConfiguration startupConf)
        {
            var defaultUris = startupConf["ASPNETCORE_URLS"];
            if (!string.IsNullOrEmpty(defaultUris))
            {
                var urls = defaultUris.Split(';', StringSplitOptions.RemoveEmptyEntries);
                var httpPort = -1;
                var httpsPort = -1;
                foreach (var url in urls)
                {
                    try
                    {
                        var uri = new Uri(url);
                        if (uri.Scheme == "http") httpPort = uri.Port;
                        else if (uri.Scheme == "https") httpsPort = uri.Port;
                    }
                    catch (Exception) { }
                }

                if (httpPort != -1 || httpsPort != -1)
                {
                    return (httpPort, httpPort);
                }
            }

            return GetCustomConfigPorts(startupConf);

        }

        private static (int http, int https) GetCustomConfigPorts(IConfiguration startupConf)
        {
            var httpPort = int.TryParse(startupConf["Ports:http"], out var hp) ? hp : -1;
            var httpsPort = int.TryParse(startupConf["Ports:https"], out var hps) ? hps : -1;
            return (httpPort, httpsPort);
        }
    }
}
