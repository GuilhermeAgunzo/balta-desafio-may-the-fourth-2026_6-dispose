using Dispose.AI.Services;
using Dispose.Core.DTOs;
using Dispose.UnitTests.Support;

namespace Dispose.UnitTests;

public sealed class WasteAssistantAgentTests
{
    [Fact]
    public async Task AskAsync_includes_context_and_returns_default_actions()
    {
        var agentInvoker = new FakeAgentInvoker { ResponseText = "Leve as pilhas ao ponto especial." };
        var contextService = new FakeWasteAssistantContextService { Context = "Bairro ativo: Coruscant Centro." };
        var agent = new WasteAssistantAgent(
            agentInvoker,
            contextService,
            new FrozenTimeProvider(new DateTimeOffset(2026, 5, 12, 14, 0, 0, TimeSpan.Zero)));

        var response = await agent.AskAsync(
            new AssistantRequestDto("Onde descarto pilhas?", "Coruscant Centro"),
            CancellationToken.None);

        Assert.Contains("Coruscant Centro", agentInvoker.LastPrompt);
        Assert.Contains("Leve as pilhas", response.Answer);
        Assert.Equal(3, response.SuggestedActions.Count);
    }
}
