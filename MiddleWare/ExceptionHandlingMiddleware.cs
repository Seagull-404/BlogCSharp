using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlogCSharp.DTOs;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

namespace BlogCSharp.MiddleWare;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private  readonly IWebHostEnvironment _env;
    
    //1.构造函数  -  注入依赖
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostingEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }
    
    //2.核心方法  -  处理请求
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {   
          await _next(context);
        }
        catch (Exception ex)    
        {
            await HandleExceptionAsync(context,ex);
        }

    }
    
    //3.异常处理方法
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // 1. 【定状态码】根据异常类型，决定返回给前端的 HTTP 状态码
        // 默认先设为 500
        var statusCode = HttpStatusCode.InternalServerError;

        var message = "服务器内部错误！";
        
        string? details = null;
        
        //2. 【异常分类】针对不同的异常类型，做不同的处理

        switch (exception)
        {
            case Exceptions.BusinessException be:
                statusCode = HttpStatusCode.BadRequest; //400
                message = be.Message; // 业务异常的消息是安全的，可以直接给前端看
                break;

            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized; //401
                message = "未授权，请登录！";
                break;

            case Exceptions.NotFoundException ne:
                //找不到异常资源
                statusCode = HttpStatusCode.NotFound;//404
                message = ne.Message;
                break;


            
            default:
                // 未知异常（系统Bug）：这是需要排查的错误
                // 详细记录堆栈信息

                _logger.LogError(exception, "全局异常捕获: {Message}", exception.Message);

                // 如果是开发环境，把堆栈信息暴露给前端方便调试
                if (_env.IsDevelopment())
                {
                    message = exception.Message;
                    details = exception.StackTrace;
                }
                else
                {
                    // 生产环境隐藏具体错误
                    message = "服务器内部错误，请联系管理员";
                }

                break;
        }

        // 3. 【构造响应对象】
        
        context.Response.StatusCode = (int)statusCode;

        var response = new ApiResponseDto()
        {
            StatusCode = (int)statusCode,
            Message = message,
            // 只有在开发环境才返回具体的堆栈信息，生产环境严禁暴露
            Details = details
        };
        
        //4.序列化并写入响应
        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            
        };
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        
        
        
    }

}
    