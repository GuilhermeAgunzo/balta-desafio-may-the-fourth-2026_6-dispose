using Dispose.Application.Services;
using Dispose.Core.DTOs;
using Dispose.Core.Entities;
using Dispose.Core.Enums;
using Dispose.UnitTests.Support;

namespace Dispose.UnitTests;

public sealed class ReminderServiceTests
{
    [Fact]
    public async Task CreateReminderAsync_rejects_collection_point_without_requested_category()
    {
        var point = new CollectionPoint
        {
            Id = Guid.NewGuid(),
            Name = "Hangar",
            NeighborhoodName = "Coruscant Centro",
            Address = "Rua 1",
            Landmark = "Base",
            FactionTag = "Alianca",
            Latitude = -23.55,
            Longitude = -46.63,
            AcceptedCategories =
            [
                new CollectionPointAcceptedCategory { CollectionPointId = Guid.NewGuid(), Category = SpecialItemCategory.Batteries }
            ]
        };

        var catalogRepository = new InMemoryCatalogRepository();
        catalogRepository.CollectionPoints.Add(point);

        var service = new ReminderService(
            catalogRepository,
            new InMemoryReminderRepository(),
            new GeoDistanceCalculator(),
            new FrozenTimeProvider(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateReminderAsync(
            new CreateReminderRequestDto("Piloto", SpecialItemCategory.Medication, point.Id, 0.8),
            CancellationToken.None));
    }

    [Fact]
    public async Task CheckNearbyAsync_returns_alert_and_updates_last_triggered()
    {
        var reminderRepository = new InMemoryReminderRepository();
        reminderRepository.Reminders.Add(
            new ReminderSubscription
            {
                Id = Guid.NewGuid(),
                ResidentAlias = "Piloto",
                Category = SpecialItemCategory.Batteries,
                CollectionPointId = Guid.NewGuid(),
                CollectionPointName = "Hangar Verde",
                CollectionPointAddress = "Rua 1",
                TargetLatitude = -23.55,
                TargetLongitude = -46.63,
                RadiusKm = 1.2,
                BrowserNotificationsEnabled = true,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-1)
            });

        var service = new ReminderService(
            new InMemoryCatalogRepository(),
            reminderRepository,
            new GeoDistanceCalculator(),
            new FrozenTimeProvider(new DateTimeOffset(2026, 5, 12, 14, 0, 0, TimeSpan.Zero)));

        var alerts = await service.CheckNearbyAsync(new ProximityCheckRequestDto(-23.5504, -46.6311), CancellationToken.None);

        Assert.Single(alerts);
        Assert.NotNull(reminderRepository.Reminders[0].LastTriggeredAt);
    }
}
