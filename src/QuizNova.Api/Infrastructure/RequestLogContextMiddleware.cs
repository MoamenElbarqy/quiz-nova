using Serilog;
using Serilog.Context;

namespace MechanicShop.Api.Infrastructure;

public class RequestLogContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        // It Returns IDisposable So We Should Dispose it or using a scope to call dispose automatilly
        using (LogContext.PushProperty("CorrelationId", httpContext.TraceIdentifier))
        {
            // the purpose is pushing the request correlation id into the log context
            // to be included in the structured log of a life time of http request
            Log.Information(
                "Request Started {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
            await next(httpContext);
            Log.Information("Request Finished");
        }
    }
}