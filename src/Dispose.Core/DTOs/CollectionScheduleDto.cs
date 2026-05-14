using Dispose.Core.Enums;

namespace Dispose.Core.DTOs;

public sealed record CollectionScheduleDto(
    Guid Id,
    Guid NeighborhoodId,
    string NeighborhoodName,
    WasteType WasteType,
    string WasteTypeLabel,
    DayOfWeek DayOfWeek,
    string DayLabel,
    DateOnly NextCollectionDate,
    string PickupWindow,
    string RouteCode,
    string Guidance);
