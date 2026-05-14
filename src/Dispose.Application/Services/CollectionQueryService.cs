using Dispose.Application.Abstractions;
using Dispose.Core.DTOs;
using Dispose.Core.Entities;
using Dispose.Core.Enums;
using Dispose.Core.Extensions;

namespace Dispose.Application.Services;

public sealed class CollectionQueryService : ICollectionQueryService
{
    private readonly IDisposeCatalogRepository _catalogRepository;
    private readonly IReminderRepository _reminderRepository;
    private readonly IGeoDistanceCalculator _geoDistanceCalculator;
    private readonly TimeProvider _timeProvider;

    public CollectionQueryService(
        IDisposeCatalogRepository catalogRepository,
        IReminderRepository reminderRepository,
        IGeoDistanceCalculator geoDistanceCalculator,
        TimeProvider timeProvider)
    {
        _catalogRepository = catalogRepository;
        _reminderRepository = reminderRepository;
        _geoDistanceCalculator = geoDistanceCalculator;
        _timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<NeighborhoodSummaryDto>> GetNeighborhoodsAsync(CancellationToken cancellationToken)
    {
        var neighborhoods = await _catalogRepository.ListNeighborhoodsAsync(cancellationToken);

        return neighborhoods
            .OrderBy(neighborhood => neighborhood.Name)
            .Select(neighborhood => new NeighborhoodSummaryDto(
                neighborhood.Id,
                neighborhood.Name,
                neighborhood.Sector,
                neighborhood.Motto,
                neighborhood.Latitude,
                neighborhood.Longitude))
            .ToArray();
    }

    public async Task<DashboardOverviewDto> GetDashboardAsync(string neighborhoodName, CancellationToken cancellationToken)
    {
        var neighborhood = await _catalogRepository.GetNeighborhoodByNameAsync(neighborhoodName, cancellationToken)
            ?? throw new KeyNotFoundException($"Bairro '{neighborhoodName}' nao foi encontrado.");

        var schedules = await _catalogRepository.ListSchedulesAsync(neighborhoodName, null, cancellationToken);
        var collectionPoints = await _catalogRepository.ListCollectionPointsAsync(neighborhoodName, null, cancellationToken);
        var reminders = await _reminderRepository.ListActiveAsync(cancellationToken);

        var upcomingCollections = schedules
            .OrderBy(schedule => CalculateNextCollectionDate(schedule.DayOfWeek))
            .Take(4)
            .Select(MapUpcomingCollection)
            .ToArray();

        var featuredPoints = collectionPoints
            .Select(point => MapCollectionPoint(point, neighborhood.Latitude, neighborhood.Longitude))
            .OrderBy(point => point.DistanceKm ?? double.MaxValue)
            .Take(3)
            .ToArray();

        return new DashboardOverviewDto(
            neighborhood.Name,
            neighborhood.Sector,
            neighborhood.Motto,
            upcomingCollections,
            featuredPoints,
            reminders.Count,
            _timeProvider.GetUtcNow());
    }

    public async Task<IReadOnlyList<CollectionScheduleDto>> GetSchedulesAsync(
        string? neighborhoodName,
        WasteType? wasteType,
        CancellationToken cancellationToken)
    {
        var schedules = await _catalogRepository.ListSchedulesAsync(neighborhoodName, wasteType, cancellationToken);

        return schedules
            .OrderBy(schedule => schedule.Neighborhood?.Name)
            .ThenBy(schedule => CalculateNextCollectionDate(schedule.DayOfWeek))
            .ThenBy(schedule => schedule.WasteType)
            .Select(MapCollectionSchedule)
            .ToArray();
    }

    public async Task<IReadOnlyList<CollectionPointDto>> GetCollectionPointsAsync(
        string? neighborhoodName,
        SpecialItemCategory? category,
        double? latitude,
        double? longitude,
        CancellationToken cancellationToken)
    {
        var points = await _catalogRepository.ListCollectionPointsAsync(neighborhoodName, category, cancellationToken);
        Neighborhood? neighborhood = null;

        if (!string.IsNullOrWhiteSpace(neighborhoodName))
        {
            neighborhood = await _catalogRepository.GetNeighborhoodByNameAsync(neighborhoodName, cancellationToken);
        }

        var referenceLatitude = latitude ?? neighborhood?.Latitude;
        var referenceLongitude = longitude ?? neighborhood?.Longitude;

        return points
            .Select(point => MapCollectionPoint(point, referenceLatitude, referenceLongitude))
            .OrderBy(point => point.DistanceKm ?? double.MaxValue)
            .ThenBy(point => point.Name)
            .ToArray();
    }

    private UpcomingCollectionDto MapUpcomingCollection(CollectionSchedule schedule) =>
        new(
            schedule.Id,
            schedule.WasteType,
            schedule.WasteType.ToDisplayName(),
            schedule.DayOfWeek,
            schedule.DayOfWeek.ToDisplayName(),
            CalculateNextCollectionDate(schedule.DayOfWeek),
            schedule.PickupWindow,
            schedule.RouteCode,
            schedule.Guidance);

    private CollectionScheduleDto MapCollectionSchedule(CollectionSchedule schedule) =>
        new(
            schedule.Id,
            schedule.NeighborhoodId,
            schedule.Neighborhood?.Name ?? string.Empty,
            schedule.WasteType,
            schedule.WasteType.ToDisplayName(),
            schedule.DayOfWeek,
            schedule.DayOfWeek.ToDisplayName(),
            CalculateNextCollectionDate(schedule.DayOfWeek),
            schedule.PickupWindow,
            schedule.RouteCode,
            schedule.Guidance);

    private CollectionPointDto MapCollectionPoint(CollectionPoint point, double? referenceLatitude, double? referenceLongitude)
    {
        double? distanceKm = null;

        if (referenceLatitude.HasValue && referenceLongitude.HasValue)
        {
            distanceKm = Math.Round(
                _geoDistanceCalculator.CalculateKilometers(
                    referenceLatitude.Value,
                    referenceLongitude.Value,
                    point.Latitude,
                    point.Longitude),
                2);
        }

        var acceptedCategories = point.AcceptedCategories
            .Select(category => category.Category)
            .OrderBy(category => category)
            .ToArray();

        return new CollectionPointDto(
            point.Id,
            point.Name,
            point.NeighborhoodName,
            point.Address,
            point.Landmark,
            point.FactionTag,
            point.Latitude,
            point.Longitude,
            acceptedCategories,
            acceptedCategories.Select(category => category.ToDisplayName()).ToArray(),
            distanceKm);
    }

    private DateOnly CalculateNextCollectionDate(DayOfWeek dayOfWeek)
    {
        var currentDate = DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);
        var delta = ((int)dayOfWeek - (int)currentDate.DayOfWeek + 7) % 7;
        return currentDate.AddDays(delta == 0 ? 7 : delta);
    }
}
