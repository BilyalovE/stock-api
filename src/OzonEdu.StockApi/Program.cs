// Содержит классы и интерфейсы для запуска и настройки веб-приложения

using Microsoft.AspNetCore.Hosting;
// Инфраструктура для управления жизненным циклом приложения
using Microsoft.Extensions.Hosting;
using System.Net;  
using Microsoft.AspNetCore.Server.Kestrel.Core;
using OzonEdu.StockApi;
using OzonEdu.StockApi.Infrastructure.Extensions;


CreateHostBuilder(args).Build().Run(); 

static IHostBuilder CreateHostBuilder(string[] args)
    => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(
            webBuilder =>
            {
                webBuilder.ConfigureKestrel(options =>
                {
                    options.Listen(IPAddress.Any, 5001, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2; // Устанавливаем протокол HTTP/2
                    });
                });
                webBuilder.UseStartup<Startup>();
            })
        .AddInfrastructure()
        .AddHttp();
