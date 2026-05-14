using Dispose.Core.Enums;

namespace Dispose.Core.DTOs;

public sealed record ReminderDto(
    Guid Id,
    string ResidentAlias,
    SpecialItemCategory Category,
    string CategoryLabel,
    Guid CollectionPointId,
    string CollectionPointName,
    string CollectionPointAddress,
    double TargetLatitude,
    double TargetLongitude,
    double RadiusKm,
    bool BrowserNotificationsEnabled,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastTriggeredAt);
