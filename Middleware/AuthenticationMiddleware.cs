using System.Net;
using System.Text.Json;

namespace UserManagementAPI.Middleware;

/// <summary>
/// Enforces token-based authentication by validating the Bearer token on each request.
/// </summary>
public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;
    private readonly string _validToken;

    public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _validToken = configuration["Auth:Token"] ?? "techhive-secret-token";
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? authHeader = context.Request.Headers.Authorization;

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            await WriteUnauthorizedAsync(context, "Missing or malformed Authorization header.");
            return;
        }

        var token = authHeader["Bearer ".Length..].Trim();

        if (!string.Equals(token, _validToken, StringComparison.Ordinal))
        {
            _logger.LogWarning("Rejected request with invalid token for {Path}", context.Request.Path);
            await WriteUnauthorizedAsync(context, "Invalid token.");
            return;
        }

        await _next(context);
    }

    private static async Task WriteUnauthorizedAsync(HttpContext context, string message)
    {
        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

        var payload = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(payload);
    }
}
