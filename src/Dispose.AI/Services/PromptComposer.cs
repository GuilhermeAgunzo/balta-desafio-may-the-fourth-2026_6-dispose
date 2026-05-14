using Dispose.Core.DTOs;

namespace Dispose.AI.Services;

internal static class PromptComposer
{
    public static IReadOnlyList<string> DefaultActions { get; } =
    [
        "Abrir a pagina de descarte especial",
        "Conferir a agenda de coleta do bairro",
        "Ativar um lembrete por proximidade"
    ];

    public static string SystemInstructions =>
        """
        Voce e o Dispose, um copiloto de descarte urbano com carisma de centro de comando Star Wars.
        Responda sempre em portugues do Brasil.
        Seja objetivo, pratico e confiavel.
        Quando falar de itens especiais, diferencie pilhas, eletronicos, medicamentos e oleo de cozinha.
        Quando existir um bairro ativo no contexto, use essa informacao para orientar agendas e pontos de coleta.
        Se a pergunta nao puder ser respondida com total precisao, admita a incerteza e direcione o usuario para a tela mais adequada do app.
        Evite inventar normas legais ou horarios nao presentes no contexto.
        """;

    public static string BuildUserPrompt(AssistantRequestDto request, string context)
    {
        var activeNeighborhood = string.IsNullOrWhiteSpace(request.NeighborhoodName)
            ? "nao definido"
            : request.NeighborhoodName.Trim();

        return $"""
        Contexto operacional do app:
        {context}

        Bairro em foco: {activeNeighborhood}
        Pergunta do usuario: {request.Question.Trim()}

        Estruture a resposta em ate 3 blocos curtos:
        1. Resposta principal
        2. Proximo passo dentro do app
        3. Alerta ou dica de seguranca, se houver
        """;
    }
}
