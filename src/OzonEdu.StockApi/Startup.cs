// Содержит классы и методы для настройки HTTP-конвейера (pipeline): настройки middleware в Program.cs,
// а также для определения маршрутов и конечных точек (endpoints) в веб-приложении.

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

// Предоставляет инструменты для внедрения зависимостей (Dependency Injection, DI).
// Нужно, чтобы управлять зависимостями через контейнер сервисов.
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OzonEdu.StockApi.HttpClients;
using OzonEdu.StockApi.Services;
using OzonEdu.StockApi.Configuration.Middlewares;


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
        // IApplicationBuilder app — это объект, который управляет пайплайном обработки запроса.
        public void Configure(IApplicationBuilder app)
        {
            
            app.Map("/version", builder => builder.UseMiddleware<VersionMiddlewares>());
            app.UseMiddleware<RequestLoggingMiddlewares>();
          

            // это middleware в ASP.NET Core, который включает систему маршрутизации
            // и определяет, какой обработчик (endpoint) должен обработать входящий HTTP-запрос.
            
            app.UseRouting();
            // app.UseEndpoints(...) — включает маршрутизацию и указывает, какие конечные точки (endpoints) будут доступны.
            // endpoints.MapControllers(); — включает API-контроллеры, зарегистрированные в проекте.
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }

    public class SwaggerStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                next(app);
            };
        }
    }

    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddInfrastructure(this IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // получаем возможность работать с middleware через фильтры 
                services.AddSingleton<IStartupFilter, SwaggerStartupFilter>(); 
                
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "OzonEdu.StockApi", Version = "1.0"});
                    options.CustomSchemaIds(x => x.FullName);
                
                    // Добавление доки в swagger.
                    var xmlFileName = Assembly.GetEntryAssembly().GetName().Name + ".xml";
                    var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
                    options.IncludeXmlComments(xmlFilePath);
                
                    // Используй OperationFilter 
                    options.OperationFilter<HeaderOperationFilter>();
                });
            });
            return builder;
        }
    }
}

