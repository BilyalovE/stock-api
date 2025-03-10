using OzonEdu.StockApi.Configuration.Middlewares;

namespace OzonEdu.StockApi.Infrastructure.StartupFilters;

public class VersionStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.Map("/version", builder => builder.UseMiddleware<VersionMiddlewares>());
            next(app);
        };
    }
}