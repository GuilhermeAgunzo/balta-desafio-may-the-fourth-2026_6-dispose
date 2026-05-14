using Dispose.Application.Services;
using Dispose.Core.Entities;
using Dispose.Core.Enums;
using Dispose.UnitTests.Support;

namespace Dispose.UnitTests;

public sealed class CollectionQueryServiceTests
{
    [Fact]
    public async Task GetDashboardAsync_orders_upcoming_collections_by_next_occurrence()
    {
        var neighborhood = new Neighborhood
        {
            Id = Guid.NewGuid(),
            Name = "Coruscant Centro",
            Sector = "Setor Senado",
            Motto = "Teste",
            Latitude = -23.55,
            Longitude = -46.63
        };

        var catalogRepository = new InMemoryCatalogRepository();
        catalogRepository.Neighborhoods.Add(neighborhood);
        catalogRepository.Schedules.AddRange(
        [
            new CollectionSchedule
            {
                Id = Guid.NewGuid(),
                NeighborhoodId = neighborhood.Id,
                Neighborhood = neighborhood,
                WasteType = WasteType.Glass,
                DayOfWeek = DayOfWeek.Wednesday,
                PickupWindow = "08:00-10:00",
                RouteCode = "ROT-2",
                Guidance = "Vidros embalados"
            },
            new CollectionSchedule
            {
                Id = Guid.NewGuid(),
                NeighborhoodId = neighborhood.Id,
                Neighborhood = neighborhood,
                WasteType = WasteType.Organic,
                DayOfWeek = DayOfWeek.Monday,
                PickupWindow = "06:00-08:00",
                RouteCode = "ROT-1",
                Guidance = "Organicos fechados"
            }
        ]);

        var service = new CollectionQueryService(
            catalogRepository,
            new InMemoryReminderRepository(),
            new GeoDistanceCalculator(),
            new FrozenTimeProvider(new DateTimeOffset(2026, 5, 10, 12, 0, 0, TimeSpan.Zero)));

        var dashboard = await service.GetDashboardAsync(neighborhood.Name, CancellationToken.None);

        Assert.Equal(WasteType.Organic, dashboard.UpcomingCollections[0].WasteType);
        Assert.Equal(WasteType.Glass, dashboard.UpcomingCollections[1].WasteType);
    }
}
