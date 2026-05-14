using Dispose.AI.Abstractions;
using Dispose.Application.Abstractions;
using Dispose.Core.Entities;
using Dispose.Core.Enums;

namespace Dispose.UnitTests.Support;

internal sealed class InMemoryCatalogRepository : IDisposeCatalogRepository
{
    public List<Neighborhood> Neighborhoods { get; } = [];
    public List<CollectionSchedule> Schedules { get; } = [];
    public List<CollectionPoint> CollectionPoints { get; } = [];

    public Task<IReadOnlyList<Neighborhood>> ListNeighborhoodsAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Neighborhood>>(Neighborhoods);

    public Task<Neighborhood?> GetNeighborhoodByNameAsync(string neighborhoodName, CancellationToken cancellationToken) =>
        Task.FromResult(Neighborhoods.FirstOrDefault(item =>
            string.Equals(item.Name, neighborhoodName, StringComparison.OrdinalIgnoreCase)));

    public Task<IReadOnlyList<CollectionSchedule>> ListSchedulesAsync(
        string? neighborhoodName,
        WasteType? wasteType,
        CancellationToken cancellationToken)
    {
        IEnumerable<CollectionSchedule> query = Schedules;

        if (!string.IsNullOrWhiteSpace(neighborhoodName))
        {
            query = query.Where(item =>
                string.Equals(item.Neighborhood?.Name, neighborhoodName, StringComparison.OrdinalIgnoreCase));
        }

        if (wasteType.HasValue)
        {
            query = query.Where(item => item.WasteType == wasteType.Value);
        }

        return Task.FromResult<IReadOnlyList<CollectionSchedule>>(query.ToArray());
    }

    public Task<IReadOnlyList<CollectionPoint>> ListCollectionPointsAsync(
        string? neighborhoodName,
        SpecialItemCategory? category,
        CancellationToken cancellationToken)
    {
        IEnumerable<CollectionPoint> query = CollectionPoints;

        if (!string.IsNullOrWhiteSpace(neighborhoodName))
        {
            query = query.Where(item =>
                string.Equals(item.NeighborhoodName, neighborhoodName, StringComparison.OrdinalIgnoreCase));
        }

        if (category.HasValue)
        {
            query = query.Where(item => item.AcceptedCategories.Any(entry => entry.Category == category.Value));
        }

        return Task.FromResult<IReadOnlyList<CollectionPoint>>(query.ToArray());
    }

    public Task<CollectionPoint?> GetCollectionPointAsync(Guid collectionPointId, CancellationToken cancellationToken) =>
        Task.FromResult(CollectionPoints.FirstOrDefault(item => item.Id == collectionPointId));
}

internal sealed class InMemoryReminderRepository : IReminderRepository
{
    public List<ReminderSubscription> Reminders { get; } = [];

    public Task<IReadOnlyList<ReminderSubscription>> ListActiveAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<ReminderSubscription>>(Reminders);

    public Task AddAsync(ReminderSubscription reminderSubscription, CancellationToken cancellationToken)
    {
        Reminders.Add(reminderSubscription);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

internal sealed class FrozenTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _utcNow;

    public FrozenTimeProvider(DateTimeOffset utcNow)
    {
        _utcNow = utcNow;
    }

    public override DateTimeOffset GetUtcNow() => _utcNow;

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}

internal sealed class FakeAgentInvoker : IAgentInvoker
{
    public string ResponseText { get; set; } = "Resposta sintetica";
    public string? LastPrompt { get; private set; }
    public bool IsConfigured => true;

    public Task<string> RunAsync(string prompt, CancellationToken cancellationToken)
    {
        LastPrompt = prompt;
        return Task.FromResult(ResponseText);
    }
}

internal sealed class FakeWasteAssistantContextService : IWasteAssistantContextService
{
    public string Context { get; set; } = "Contexto padrao";

    public Task<string> BuildContextAsync(string? neighborhoodName, CancellationToken cancellationToken) =>
        Task.FromResult(Context);
}
