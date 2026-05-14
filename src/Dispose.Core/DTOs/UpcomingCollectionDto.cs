using Dispose.Core.Enums;

namespace Dispose.Core.DTOs;

public sealed record UpcomingCollectionDto(
    Guid ScheduleId,
    WasteType WasteType,
    string WasteTypeLabel,
    DayOfWeek DayOfWeek,
    string DayLabel,
    DateOnly NextCollectionDate,
    string PickupWindow,
    string RouteCode,
    string Guidance);
