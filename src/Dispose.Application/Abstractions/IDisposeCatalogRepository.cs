using Dispose.Core.Entities;
using Dispose.Core.Enums;

namespace Dispose.Application.Abstractions;

public interface IDisposeCatalogRepository
{
    Task<IReadOnlyList<Neighborhood>> ListNeighborhoodsAsync(CancellationToken cancellationToken);
    Task<Neighborhood?> GetNeighborhoodByNameAsync(string neighborhoodName, CancellationToken cancellationToken);
    Task<IReadOnlyList<CollectionSchedule>> ListSchedulesAsync(
        string? neighborhoodName,
        WasteType? wasteType,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<CollectionPoint>> ListCollectionPointsAsync(
        string? neighborhoodName,
        SpecialItemCategory? category,
        CancellationToken cancellationToken);

    Task<CollectionPoint?> GetCollectionPointAsync(Guid collectionPointId, CancellationToken cancellationToken);
}
