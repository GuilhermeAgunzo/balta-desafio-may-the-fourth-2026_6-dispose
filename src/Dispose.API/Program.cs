using System.Text.Json.Serialization;
using Dispose.AI.Abstractions;
using Dispose.AI.Configuration;
using Dispose.AI.Services;
using Dispose.API.Endpoints;
using Dispose.Application.Abstractions;
using Dispose.Application.Services;
using Dispose.Infra.Persistence;
using Dispose.Infra.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "DisposeBrowser",
        policy => policy
            .SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                {
                    return false;
                }

                return uri.Host is "localhost" or "127.0.0.1";
            })
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var connectionString = builder.Configuration.GetConnectionString("DisposeDb") ?? "Data Source=dispose.db";

builder.Services.AddDbContext<DisposeDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<DisposeDbInitializer>();

builder.Services.AddScoped<IDisposeCatalogRepository, DisposeCatalogRepository>();
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IGeoDistanceCalculator, GeoDistanceCalculator>();
builder.Services.AddScoped<ICollectionQueryService, CollectionQueryService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<IWasteAssistantContextService, WasteAssistantContextService>();

builder.Services.AddSingleton(new OpenAiOptions
{
    ApiKey = builder.Configuration["OpenAI:ApiKey"] ?? string.Empty,
    Model = builder.Configuration["OpenAI:Model"] ?? "gpt-4.1-mini"
});

builder.Services.AddScoped<IAgentInvoker, OpenAiAgentInvoker>();
builder.Services.AddScoped<IWasteAssistantAgent, WasteAssistantAgent>();

var app = builder.Build();

app.UseExceptionHandler(new ExceptionHandlerOptions
{
    StatusCodeSelector = exception => exception switch
    {
        ArgumentException => StatusCodes.Status400BadRequest,
        KeyNotFoundException => StatusCodes.Status404NotFound,
        InvalidOperationException => StatusCodes.Status503ServiceUnavailable,
        _ => StatusCodes.Status500InternalServerError
    }
});

app.UseCors("DisposeBrowser");
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

await using (var scope = app.Services.CreateAsyncScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DisposeDbInitializer>();
    await initializer.InitializeAsync(CancellationToken.None);
}

app.MapDisposeApi();

app.Run();

public partial class Program;
