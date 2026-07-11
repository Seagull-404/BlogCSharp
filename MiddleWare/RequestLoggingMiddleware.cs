using Serilog.Context;
using System.Diagnostics;

namespace BlogCSharp.MiddleWare
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = Guid.NewGuid().ToString();
            var userId = context.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "Anonymous";
            var stopwatch = Stopwatch.StartNew();

            using (LogContext.PushProperty("RequestId", requestId))
            using (LogContext.PushProperty("UserId", userId))
            {
                _logger.LogInformation(
                    "Request started: {Method} {Path} - RequestId: {RequestId}, UserId: {UserId}",
                    context.Request.Method,
                    context.Request.Path,
                    requestId,
                    userId
                );

                try
                {
                    await _next(context);

                    stopwatch.Stop();
                    var elapsed = stopwatch.ElapsedMilliseconds;

                    _logger.LogInformation(
                        "Request completed: {Method} {Path} - StatusCode: {StatusCode}, Duration: {Duration}ms, RequestId: {RequestId}",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        elapsed,
                        requestId
                    );
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    var elapsed = stopwatch.ElapsedMilliseconds;

                    _logger.LogError(
                        ex,
                        "Request failed: {Method} {Path} - Duration: {Duration}ms, RequestId: {RequestId}",
                        context.Request.Method,
                        context.Request.Path,
                        elapsed,
                        requestId
                    );

                    throw;
                }
            }
        }
    }
}