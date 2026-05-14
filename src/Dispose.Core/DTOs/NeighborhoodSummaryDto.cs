namespace Dispose.Core.DTOs;

public sealed record NeighborhoodSummaryDto(
    Guid Id,
    string Name,
    string Sector,
    string Motto,
    double Latitude,
    double Longitude);
