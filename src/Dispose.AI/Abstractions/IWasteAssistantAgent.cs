using Dispose.Core.DTOs;

namespace Dispose.AI.Abstractions;

public interface IWasteAssistantAgent
{
    Task<AssistantResponseDto> AskAsync(AssistantRequestDto request, CancellationToken cancellationToken);
}
