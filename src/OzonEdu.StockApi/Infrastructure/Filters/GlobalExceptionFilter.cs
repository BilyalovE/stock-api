using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OzonEdu.StockApi.Infrastructure.Filters;

public class GlobalExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }
    public override void OnException(ExceptionContext context)
    {
        base.OnException(context);
        var resultObject = new
        {
            ExceptionType = context.Exception.GetType().FullName,
            Message = context.Exception.Message,
        };

        var jsonResult = new JsonResult(resultObject)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        _logger.LogError(context.Exception.Message);
        context.Result = jsonResult;
    }
}