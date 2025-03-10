using Microsoft.OpenApi.Models;
using System.Reflection;
using OzonEdu.StockApi.Infrastructure.StartupFilters;
using OzonEdu.StockApi.Infrastructure.Filters;



namespace OzonEdu.StockApi.Infrastructure.Extensions;

public static partial class HostBuilderExtensions
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
    
    public static IHostBuilder AddHttp(this IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Регистрируем поддержку API-контроллеров в ASP.NET Core.  
            services.AddControllers(options => options.Filters.Add<GlobalExceptionFilter>());
        });
        return builder;
    }
}
