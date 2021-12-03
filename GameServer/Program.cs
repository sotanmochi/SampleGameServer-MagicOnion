using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Grpc.Core;

namespace GameServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel(options =>
                        {
                            // WORKAROUND: Accept HTTP/2 only to allow insecure HTTP/2 connections during development.
                            options.ConfigureEndpointDefaults(endpointOptions =>
                            {
                                endpointOptions.Protocols = HttpProtocols.Http2;
                            });
                        })
                        .UseStartup<Startup>();
                });
    }
}