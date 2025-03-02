// Содержит классы и методы для настройки HTTP-конвейера (pipeline): настройки middleware в Program.cs,
// а также для определения маршрутов и конечных точек (endpoints) в веб-приложении.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

// Предоставляет инструменты для внедрения зависимостей (Dependency Injection, DI).
// Нужно, чтобы управлять зависимостями через контейнер сервисов.
using Microsoft.Extensions.DependencyInjection;
using OzonEdu.StockApi.HttpClients;
using OzonEdu.StockApi.Services;

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
            // Регистрируем поддержку API-контроллеров в ASP.NET Core.
            services.AddControllers();
            services.AddSingleton<IStockService, StockService>();

            services.AddHttpClient<IStockApiHttpClient, StockApiHttpClient>();
        }

        // Здесь мы конфигурируем само приложение, в данном случае пайплайн обработки запроса
        public void Configure(IApplicationBuilder app)
        {
            // IApplicationBuilder app — это объект, который управляет пайплайном обработки запроса.

            // это middleware в ASP.NET Core, который включает систему маршрутизации
            // и определяет, какой обработчик (endpoint) должен обработать входящий HTTP-запрос.
            app.UseRouting();

            // app.UseEndpoints(...) — включает маршрутизацию и указывает, какие конечные точки (endpoints) будут доступны.
            // endpoints.MapControllers(); — включает API-контроллеры, зарегистрированные в проекте.
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}