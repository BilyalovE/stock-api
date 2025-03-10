using Microsoft.OpenApi.Models;
using System.Reflection;


namespace OzonEdu.StockApi.Infrastructure;

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
