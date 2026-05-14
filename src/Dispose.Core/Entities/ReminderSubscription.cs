using Dispose.Core.Enums;

namespace Dispose.Core.Entities;

public sealed class ReminderSubscription
{
    public Guid Id { get; set; }
    public string ResidentAlias { get; set; } = string.Empty;
    public SpecialItemCategory Category { get; set; }
    public Guid CollectionPointId { get; set; }
    public string CollectionPointName { get; set; } = string.Empty;
    public string CollectionPointAddress { get; set; } = string.Empty;
    public double TargetLatitude { get; set; }
    public double TargetLongitude { get; set; }
    public double RadiusKm { get; set; }
    public bool BrowserNotificationsEnabled { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastTriggeredAt { get; set; }
}
