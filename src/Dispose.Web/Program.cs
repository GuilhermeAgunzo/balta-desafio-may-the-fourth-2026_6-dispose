using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Dispose.Core.DTOs;
using Dispose.Web;
using Dispose.Web.Models;
using Dispose.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7023";

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBaseUrl, UriKind.Absolute) });
builder.Services.AddScoped<IDisposeApiClient, DisposeApiClient>();
builder.Services.AddScoped<IUserPreferencesStore, BrowserPreferencesStore>();
builder.Services.AddScoped<IBrowserBridge, BrowserBridge>();
builder.Services.AddScoped<AlertCenter>();
builder.Services.AddScoped<IReminderWatcher, ReminderWatcher>();

await builder.Build().RunAsync();
