using Dispose.AI.Abstractions;
using Dispose.Application.Abstractions;
using Dispose.Core.DTOs;

namespace Dispose.AI.Services;

public sealed class WasteAssistantAgent : IWasteAssistantAgent
{
    private readonly IAgentInvoker _agentInvoker;
    private readonly IWasteAssistantContextService _contextService;
    private readonly TimeProvider _timeProvider;

    public WasteAssistantAgent(
        IAgentInvoker agentInvoker,
        IWasteAssistantContextService contextService,
        TimeProvider timeProvider)
    {
        _agentInvoker = agentInvoker;
        _contextService = contextService;
        _timeProvider = timeProvider;
    }

    public async Task<AssistantResponseDto> AskAsync(AssistantRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            throw new ArgumentException("Escreva uma pergunta para o assistente.", nameof(request));
        }

        var context = await _contextService.BuildContextAsync(request.NeighborhoodName, cancellationToken);
        var prompt = PromptComposer.BuildUserPrompt(request, context);
        var answer = await _agentInvoker.RunAsync(prompt, cancellationToken);

        return new AssistantResponseDto(
            request.Question.Trim(),
            answer,
            request.NeighborhoodName,
            PromptComposer.DefaultActions,
            _timeProvider.GetUtcNow());
    }
}
