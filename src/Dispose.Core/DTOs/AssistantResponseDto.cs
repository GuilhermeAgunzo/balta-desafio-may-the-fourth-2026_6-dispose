namespace Dispose.Core.DTOs;

public sealed record AssistantResponseDto(
    string Question,
    string Answer,
    string? NeighborhoodName,
    IReadOnlyList<string> SuggestedActions,
    DateTimeOffset GeneratedAt);
