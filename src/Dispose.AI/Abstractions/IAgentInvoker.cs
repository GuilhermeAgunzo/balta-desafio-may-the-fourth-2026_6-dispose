namespace Dispose.AI.Abstractions;

public interface IAgentInvoker
{
    bool IsConfigured { get; }
    Task<string> RunAsync(string prompt, CancellationToken cancellationToken);
}
