namespace Dispose.Core.Entities;

public sealed class CollectionPoint
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NeighborhoodName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Landmark { get; set; } = string.Empty;
    public string FactionTag { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public ICollection<CollectionPointAcceptedCategory> AcceptedCategories { get; set; } = [];
}
