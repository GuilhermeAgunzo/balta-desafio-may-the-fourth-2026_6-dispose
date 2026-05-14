using Dispose.Core.DTOs;
using Dispose.Core.Enums;

namespace Dispose.Application.Abstractions;

public interface ICollectionQueryService
{
    Task<IReadOnlyList<NeighborhoodSummaryDto>> GetNeighborhoodsAsync(CancellationToken cancellationToken);
    Task<DashboardOverviewDto> GetDashboardAsync(string neighborhoodName, CancellationToken cancellationToken);
    Task<IReadOnlyList<CollectionScheduleDto>> GetSchedulesAsync(
        string? neighborhoodName,
        WasteType? wasteType,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CollectionPointDto>> GetCollectionPointsAsync(
        string? neighborhoodName,
        SpecialItemCategory? category,
        double? latitude,
        double? longitude,
        CancellationToken cancellationToken);
}
