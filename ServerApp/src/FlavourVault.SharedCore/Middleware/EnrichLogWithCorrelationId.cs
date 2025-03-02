using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace FlavourVault.SharedCore.Middleware;
public sealed class EnrichLogWithCorrelationId
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public EnrichLogWithCorrelationId(RequestDelegate next)
    {
        _next = next;
    }

    private static string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(
            CorrelationIdHeaderName, out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }

    public async Task InvokeAsync(HttpContext context)
    {    
        string correlationId = GetCorrelationId(context);
        using (LogContext.PushProperty("CorrelationId", correlationId))
        await _next(context);        
    }
}
