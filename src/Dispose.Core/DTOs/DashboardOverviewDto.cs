namespace Dispose.Core.DTOs;

public sealed record DashboardOverviewDto(
    string NeighborhoodName,
    string Sector,
    string Motto,
    IReadOnlyList<UpcomingCollectionDto> UpcomingCollections,
    IReadOnlyList<CollectionPointDto> FeaturedCollectionPoints,
    int ActiveReminderCount,
    DateTimeOffset GeneratedAt);
