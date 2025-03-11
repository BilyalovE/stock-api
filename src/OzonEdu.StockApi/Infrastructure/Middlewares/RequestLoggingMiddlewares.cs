
using System.Text;
using System.Text.Json;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Identity.Data;

namespace OzonEdu.StockApi.Configuration.Middlewares;

public class RequestLoggingMiddlewares
{
    private readonly RequestDelegate _next;

    // Встроенный логгер в ASP NET Core
    private readonly ILogger<RequestLoggingMiddlewares> _logger;

    public RequestLoggingMiddlewares(RequestDelegate next, ILogger<RequestLoggingMiddlewares> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        await LogRequest(context);
        await _next(context);
    }

    private async Task LogRequest(HttpContext context)
    {
        try
        {
            // если какой-то контент в запросе есть
            if (context.Request.ContentLength > 0)
            {
                // позволяет читать тело запроса несколько раз
                context.Request.EnableBuffering();
                // размер буфера равен размеру тела запроса 
                var buffer = new byte[context.Request.ContentLength.Value];
                // читаем тело запроса 
                await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
                // байты переводим в текст 
                var bodyAsText = Encoding.UTF8.GetString(buffer);
                _logger.LogInformation(bodyAsText);
                // тело запроса будет считываться дальше в пайплайне снова с самого начала
                context.Request.Body.Position = 0;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "could not log request");
        }
    }
}

public class LoggingInterceptor : Interceptor
{
    private readonly ILogger<RequestLoggingMiddlewares> _logger;

    public LoggingInterceptor(ILogger<RequestLoggingMiddlewares> logger)
    {
        _logger = logger;
    }

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var requestJson = JsonSerializer.Serialize(request);
        _logger.LogInformation(requestJson);

        var responce = base.UnaryServerHandler(request, context, continuation);

        var responceJson = JsonSerializer.Serialize(responce);
        _logger.LogInformation(responceJson);

        return responce;
    }
}