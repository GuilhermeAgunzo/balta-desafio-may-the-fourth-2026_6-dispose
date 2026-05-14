using Dispose.Core.Enums;

namespace Dispose.Core.Entities;

public sealed class CollectionSchedule
{
    public Guid Id { get; set; }
    public Guid NeighborhoodId { get; set; }
    public WasteType WasteType { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string PickupWindow { get; set; } = string.Empty;
    public string RouteCode { get; set; } = string.Empty;
    public string Guidance { get; set; } = string.Empty;
    public Neighborhood? Neighborhood { get; set; }
}
