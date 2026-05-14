using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Dispose.API.Tests;

public sealed class DisposeApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"dispose-tests-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DisposeDb"] = $"Data Source={_databasePath}",
                    ["OpenAI:ApiKey"] = string.Empty,
                    ["OpenAI:Model"] = "gpt-4.1-mini"
                });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }
}
