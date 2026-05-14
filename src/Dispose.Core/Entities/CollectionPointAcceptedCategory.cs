using Dispose.Core.Enums;

namespace Dispose.Core.Entities;

public sealed class CollectionPointAcceptedCategory
{
    public Guid CollectionPointId { get; set; }
    public SpecialItemCategory Category { get; set; }
    public CollectionPoint? CollectionPoint { get; set; }
}
