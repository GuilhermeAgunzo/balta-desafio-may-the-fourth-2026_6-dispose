namespace Dispose.Application.Abstractions;

public interface IWasteAssistantContextService
{
    Task<string> BuildContextAsync(string? neighborhoodName, CancellationToken cancellationToken);
}
