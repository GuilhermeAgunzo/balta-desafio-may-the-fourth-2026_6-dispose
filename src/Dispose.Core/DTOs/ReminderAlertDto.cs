using Dispose.Core.Enums;

namespace Dispose.Core.DTOs;

public sealed record ReminderAlertDto(
    Guid ReminderId,
    string ResidentAlias,
    SpecialItemCategory Category,
    string CategoryLabel,
    string CollectionPointName,
    string AlertMessage,
    double DistanceKm,
    DateTimeOffset TriggeredAt);
