using Dispose.Application.Abstractions;
using Dispose.Core.Entities;
using Dispose.Core.Enums;
using Dispose.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dispose.Infra.Repositories;

public sealed class DisposeCatalogRepository : IDisposeCatalogRepository
{
    private readonly DisposeDbContext _dbContext;

    public DisposeCatalogRepository(DisposeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Neighborhood>> ListNeighborhoodsAsync(CancellationToken cancellationToken) =>
        await _dbContext.Neighborhoods
            .AsNoTracking()
            .OrderBy(neighborhood => neighborhood.Name)
            .ToListAsync(cancellationToken);

    public async Task<Neighborhood?> GetNeighborhoodByNameAsync(string neighborhoodName, CancellationToken cancellationToken)
    {
        var normalizedNeighborhoodName = neighborhoodName.Trim().ToLowerInvariant();

        return await _dbContext.Neighborhoods
            .AsNoTracking()
            .FirstOrDefaultAsync(
                neighborhood => neighborhood.Name.ToLower() == normalizedNeighborhoodName,
                cancellationToken);
    }

    public async Task<IReadOnlyList<CollectionSchedule>> ListSchedulesAsync(
        string? neighborhoodName,
        WasteType? wasteType,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.CollectionSchedules
            .AsNoTracking()
            .Include(schedule => schedule.Neighborhood)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(neighborhoodName))
        {
            var normalizedNeighborhoodName = neighborhoodName.Trim().ToLowerInvariant();
            query = query.Where(schedule =>
                schedule.Neighborhood != null &&
                schedule.Neighborhood.Name.ToLower() == normalizedNeighborhoodName);
        }

        if (wasteType.HasValue)
        {
            query = query.Where(schedule => schedule.WasteType == wasteType.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CollectionPoint>> ListCollectionPointsAsync(
        string? neighborhoodName,
        SpecialItemCategory? category,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.CollectionPoints
            .AsNoTracking()
            .Include(point => point.AcceptedCategories)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(neighborhoodName))
        {
            var normalizedNeighborhoodName = neighborhoodName.Trim().ToLowerInvariant();
            query = query.Where(point => point.NeighborhoodName.ToLower() == normalizedNeighborhoodName);
        }

        if (category.HasValue)
        {
            query = query.Where(point => point.AcceptedCategories.Any(item => item.Category == category.Value));
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<CollectionPoint?> GetCollectionPointAsync(Guid collectionPointId, CancellationToken cancellationToken) =>
        await _dbContext.CollectionPoints
            .AsNoTracking()
            .Include(point => point.AcceptedCategories)
            .FirstOrDefaultAsync(point => point.Id == collectionPointId, cancellationToken);
}
