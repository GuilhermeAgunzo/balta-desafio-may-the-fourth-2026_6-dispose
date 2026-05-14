using Bunit;
using Dispose.Core.DTOs;
using Dispose.Core.Enums;
using Dispose.Web.Pages;
using Dispose.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dispose.Web.Tests;

public sealed class HomePageTests : TestContext
{
    public HomePageTests()
    {
        Services.AddSingleton<IDisposeApiClient>(new FakeDisposeApiClient());
        Services.AddSingleton<IUserPreferencesStore>(new FakePreferencesStore());
    }

    [Fact]
    public void Home_renders_dashboard_data_from_api()
    {
        var cut = RenderComponent<Home>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Radar de coleta", cut.Markup);
            Assert.Contains("Coruscant Centro", cut.Markup);
            Assert.Contains("Vidro", cut.Markup);
        });
    }
}
