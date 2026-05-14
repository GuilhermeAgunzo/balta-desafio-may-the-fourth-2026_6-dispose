using Dispose.AI.Abstractions;
using Dispose.AI.Configuration;
using Microsoft.Agents.AI;
using OpenAI.Chat;

namespace Dispose.AI.Services;

public sealed class OpenAiAgentInvoker : IAgentInvoker
{
    private readonly OpenAiOptions _options;
    private readonly Lazy<AIAgent> _agentFactory;

    public OpenAiAgentInvoker(OpenAiOptions options)
    {
        _options = options;
        _agentFactory = new Lazy<AIAgent>(CreateAgent);
    }

    public bool IsConfigured => _options.IsConfigured;

    public async Task<string> RunAsync(string prompt, CancellationToken cancellationToken)
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException("OpenAI nao configurado. Defina OpenAI:ApiKey para habilitar o assistente.");
        }

        var response = await _agentFactory.Value.RunAsync(prompt, cancellationToken: cancellationToken);
        return response.Text.Trim();
    }

    private AIAgent CreateAgent()
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException("OpenAI nao configurado. Defina OpenAI:ApiKey para habilitar o assistente.");
        }

        ChatClient chatClient = new(model: _options.Model, apiKey: _options.ApiKey);

        return chatClient.AsAIAgent(
            name: "DisposeOracle",
            instructions: PromptComposer.SystemInstructions);
    }
}
