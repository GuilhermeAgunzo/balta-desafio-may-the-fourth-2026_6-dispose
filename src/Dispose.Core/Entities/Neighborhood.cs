using Dispose.Core.Entities;

namespace Dispose.Core.Entities;

public sealed class Neighborhood
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string Motto { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public ICollection<CollectionSchedule> CollectionSchedules { get; set; } = [];
}
