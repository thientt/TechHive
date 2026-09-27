using UserManagementAPI.Middleware;
using UserManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog to write logs to the console and a daily rolling file under /logs.
//builder.Host.UseSerilog((context, config) => config
//    .ReadFrom.Configuration(context.Configuration)
//    .WriteTo.Console()
//    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddSingleton<IUserStore, InMemoryUserStore>();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Middleware pipeline order:
// 1. Error handling first so it wraps all downstream middleware and catches every exception.
// 2. Authentication next so unauthenticated requests are rejected before reaching endpoints.
// 3. Logging last so it records only authenticated, valid requests and their responses.
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.MapControllers();

app.Run();
