using System.Reflection;

namespace OzonEdu.StockApi.Configuration.Middlewares;

public class VersionMiddlewares
{
    private readonly RequestDelegate _next;
    public VersionMiddlewares(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "no version";
        context.Response.WriteAsync(version);
        await _next(context);
    }
}