using Dispose.Core.Enums;

namespace Dispose.Core.DTOs;

public sealed record CreateReminderRequestDto(
    string ResidentAlias,
    SpecialItemCategory Category,
    Guid CollectionPointId,
    double RadiusKm,
    bool BrowserNotificationsEnabled = true);
