// Содержит классы и методы для настройки HTTP-конвейера (pipeline): настройки middleware в Program.cs,
// а также для определения маршрутов и конечных точек (endpoints) в веб-приложении.

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;


// Предоставляет инструменты для внедрения зависимостей (Dependency Injection, DI).
// Нужно, чтобы управлять зависимостями через контейнер сервисов.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OzonEdu.StockApi.Configuration.Middlewares;
using OzonEdu.StockApi.HttpClients;
using OzonEdu.StockApi.Services;
using OzonEdu.StockApi.Services.Interfaces;
using OzonEdu.StockApi.GrpcServices;



namespace OzonEdu.StockApi
{
    // Класс в котором прописана конфигурация сервисов и middleware
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Создаем 2 обязательных метода 

        // Здесь мы регистрируем все сервисы, которые будут использоваться при обработке запроса
        public void ConfigureServices(IServiceCollection services)
        {
            // По сути здесь должна регистрироваться только бизнес логика и все 
            services.AddSingleton<IStockService, StockService>(); 
            services.AddHttpClient<IStockApiHttpClient, StockApiHttpClient>();
            services.AddGrpc(options => options.Interceptors.Add<LoggingInterceptor>());
        }

        // Здесь мы конфигурируем само приложение, в данном случае пайплайн обработки запроса
        // IApplicationBuilder app — это объект, который управляет пайплайном обработки запроса.
        public void Configure(IApplicationBuilder app)
        {
            
            // это middleware в ASP.NET Core, который включает систему маршрутизации
            // и определяет, какой обработчик (endpoint) должен обработать входящий HTTP-запрос.
            
            app.UseRouting();
            // app.UseEndpoints(...) — включает маршрутизацию и указывает, какие конечные точки (endpoints) будут доступны.
            // endpoints.MapControllers(); — включает API-контроллеры, зарегистрированные в проекте.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<StockApiGrpcService>();
            });
            
        }
    }
}

