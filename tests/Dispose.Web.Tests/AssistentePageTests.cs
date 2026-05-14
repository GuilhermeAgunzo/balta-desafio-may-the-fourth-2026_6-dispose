using Bunit;
using Dispose.Web.Pages;
using Dispose.Web.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Dispose.Web.Tests;

public sealed class AssistentePageTests : TestContext
{
    public AssistentePageTests()
    {
        Services.AddSingleton<IDisposeApiClient>(new FakeDisposeApiClient { AssistantConfigured = false });
        Services.AddSingleton<IUserPreferencesStore>(new FakePreferencesStore());
    }

    [Fact]
    public void Assistente_shows_warning_when_openai_is_not_configured()
    {
        var cut = RenderComponent<Assistente>();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("OpenAI nao configurado", cut.Markup);
            Assert.Contains("Configure", cut.Markup);
        });
    }
}
