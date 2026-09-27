using System.Diagnostics;

namespace UserManagementAPI.Middleware;

/// <summary>
/// Logs every incoming request and outgoing response for auditing purposes.
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        _logger.LogInformation("Incoming request: {Method} {Path}{QueryString}",
            request.Method, request.Path, request.QueryString);

        var stopwatch = Stopwatch.StartNew();
        await _next(context);
        stopwatch.Stop();

        _logger.LogInformation("Outgoing response: {StatusCode} for {Method} {Path} in {ElapsedMs}ms",
            context.Response.StatusCode, request.Method, request.Path, stopwatch.ElapsedMilliseconds);
    }
}
