using System.Text;
using Dispose.Application.Abstractions;
using Dispose.Core.Extensions;

namespace Dispose.Application.Services;

public sealed class WasteAssistantContextService : IWasteAssistantContextService
{
    private readonly IDisposeCatalogRepository _catalogRepository;

    public WasteAssistantContextService(IDisposeCatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<string> BuildContextAsync(string? neighborhoodName, CancellationToken cancellationToken)
    {
        var neighborhoods = await _catalogRepository.ListNeighborhoodsAsync(cancellationToken);
        var selectedNeighborhood = string.IsNullOrWhiteSpace(neighborhoodName)
            ? neighborhoods.FirstOrDefault()
            : neighborhoods.FirstOrDefault(neighborhood =>
                string.Equals(neighborhood.Name, neighborhoodName, StringComparison.OrdinalIgnoreCase));

        var schedules = await _catalogRepository.ListSchedulesAsync(selectedNeighborhood?.Name, null, cancellationToken);
        var collectionPoints = await _catalogRepository.ListCollectionPointsAsync(selectedNeighborhood?.Name, null, cancellationToken);

        var builder = new StringBuilder();
        builder.AppendLine("Contexto do Dispose:");

        if (selectedNeighborhood is not null)
        {
            builder.AppendLine($"- Bairro ativo: {selectedNeighborhood.Name} ({selectedNeighborhood.Sector}).");
            builder.AppendLine($"- Tom da experiencia: {selectedNeighborhood.Motto}.");
        }
        else
        {
            builder.AppendLine("- Bairro ativo: ainda nao informado pelo usuario.");
            builder.AppendLine($"- Bairros cadastrados: {string.Join(", ", neighborhoods.Select(neighborhood => neighborhood.Name))}.");
        }

        builder.AppendLine("- Agenda de coleta disponivel:");
        foreach (var schedule in schedules.OrderBy(item => item.DayOfWeek).ThenBy(item => item.WasteType))
        {
            builder.AppendLine(
                $"  - {schedule.WasteType.ToDisplayName()} em {schedule.DayOfWeek.ToDisplayName()} ({schedule.PickupWindow}) no bairro {(schedule.Neighborhood?.Name ?? selectedNeighborhood?.Name ?? "indefinido")}.");
        }

        builder.AppendLine("- Pontos de descarte especial:");
        foreach (var point in collectionPoints.OrderBy(point => point.Name))
        {
            var categories = string.Join(", ", point.AcceptedCategories.Select(category => category.Category.ToDisplayName()));
            builder.AppendLine($"  - {point.Name} em {point.NeighborhoodName}, aceita: {categories}.");
        }

        builder.AppendLine("- Regras de orientacao:");
        builder.AppendLine("  - Pilhas e baterias nao devem ir no reciclavel comum.");
        builder.AppendLine("  - Eletronicos precisam de ponto especializado ou ecoponto.");
        builder.AppendLine("  - Medicamentos vencidos devem seguir para farmacia ou ponto de saude parceiro.");
        builder.AppendLine("  - Oleo de cozinha deve ser levado em recipiente fechado.");

        return builder.ToString();
    }
}
