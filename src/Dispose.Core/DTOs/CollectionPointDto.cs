using Dispose.Core.Enums;

namespace Dispose.Core.DTOs;

public sealed record CollectionPointDto(
    Guid Id,
    string Name,
    string NeighborhoodName,
    string Address,
    string Landmark,
    string FactionTag,
    double Latitude,
    double Longitude,
    IReadOnlyList<SpecialItemCategory> AcceptedCategories,
    IReadOnlyList<string> AcceptedCategoryLabels,
    double? DistanceKm);
