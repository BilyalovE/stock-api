using OzonEdu.StockApi.Configuration.Middlewares;

namespace OzonEdu.StockApi.Infrastructure.StartupFilters;

public class LoggingStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.UseMiddleware<RequestLoggingMiddlewares>();
            next(app);
        };
    }
}