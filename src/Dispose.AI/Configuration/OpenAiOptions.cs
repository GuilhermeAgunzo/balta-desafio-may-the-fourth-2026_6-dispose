namespace Dispose.AI.Configuration;

public sealed class OpenAiOptions
{
    public string ApiKey { get; init; } = string.Empty;
    public string Model { get; init; } = "gpt-4.1-mini";
    public bool IsConfigured => !string.IsNullOrWhiteSpace(ApiKey);
}
