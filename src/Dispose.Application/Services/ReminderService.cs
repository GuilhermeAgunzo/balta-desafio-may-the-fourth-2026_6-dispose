using Dispose.Application.Abstractions;
using Dispose.Core.DTOs;
using Dispose.Core.Entities;
using Dispose.Core.Extensions;

namespace Dispose.Application.Services;

public sealed class ReminderService : IReminderService
{
    private readonly IDisposeCatalogRepository _catalogRepository;
    private readonly IReminderRepository _reminderRepository;
    private readonly IGeoDistanceCalculator _geoDistanceCalculator;
    private readonly TimeProvider _timeProvider;

    public ReminderService(
        IDisposeCatalogRepository catalogRepository,
        IReminderRepository reminderRepository,
        IGeoDistanceCalculator geoDistanceCalculator,
        TimeProvider timeProvider)
    {
        _catalogRepository = catalogRepository;
        _reminderRepository = reminderRepository;
        _geoDistanceCalculator = geoDistanceCalculator;
        _timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<ReminderDto>> GetRemindersAsync(CancellationToken cancellationToken)
    {
        var reminders = await _reminderRepository.ListActiveAsync(cancellationToken);

        return reminders
            .OrderByDescending(reminder => reminder.CreatedAt)
            .Select(MapReminder)
            .ToArray();
    }

    public async Task<ReminderDto> CreateReminderAsync(CreateReminderRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ResidentAlias))
        {
            throw new ArgumentException("Informe um identificador para o lembrete.", nameof(request));
        }

        if (request.RadiusKm is < 0.1 or > 5)
        {
            throw new ArgumentException("O raio do lembrete deve ficar entre 0.1 km e 5 km.", nameof(request));
        }

        var collectionPoint = await _catalogRepository.GetCollectionPointAsync(request.CollectionPointId, cancellationToken)
            ?? throw new KeyNotFoundException("Ponto de coleta nao encontrado.");

        if (collectionPoint.AcceptedCategories.All(category => category.Category != request.Category))
        {
            throw new ArgumentException("O ponto selecionado nao aceita essa categoria especial.", nameof(request));
        }

        var reminder = new ReminderSubscription
        {
            Id = Guid.NewGuid(),
            ResidentAlias = request.ResidentAlias.Trim(),
            Category = request.Category,
            CollectionPointId = collectionPoint.Id,
            CollectionPointName = collectionPoint.Name,
            CollectionPointAddress = collectionPoint.Address,
            TargetLatitude = collectionPoint.Latitude,
            TargetLongitude = collectionPoint.Longitude,
            RadiusKm = Math.Round(request.RadiusKm, 2),
            BrowserNotificationsEnabled = request.BrowserNotificationsEnabled,
            CreatedAt = _timeProvider.GetUtcNow()
        };

        await _reminderRepository.AddAsync(reminder, cancellationToken);
        await _reminderRepository.SaveChangesAsync(cancellationToken);

        return MapReminder(reminder);
    }

    public async Task<IReadOnlyList<ReminderAlertDto>> CheckNearbyAsync(ProximityCheckRequestDto request, CancellationToken cancellationToken)
    {
        ValidateCoordinates(request.Latitude, request.Longitude);

        var now = _timeProvider.GetUtcNow();
        var reminders = await _reminderRepository.ListActiveAsync(cancellationToken);
        var alerts = new List<ReminderAlertDto>();
        var hasUpdates = false;

        foreach (var reminder in reminders)
        {
            var distanceKm = Math.Round(
                _geoDistanceCalculator.CalculateKilometers(
                    request.Latitude,
                    request.Longitude,
                    reminder.TargetLatitude,
                    reminder.TargetLongitude),
                2);

            if (distanceKm > reminder.RadiusKm)
            {
                continue;
            }

            if (reminder.LastTriggeredAt is not null &&
                now - reminder.LastTriggeredAt.Value < TimeSpan.FromMinutes(30))
            {
                continue;
            }

            reminder.LastTriggeredAt = now;
            hasUpdates = true;

            alerts.Add(new ReminderAlertDto(
                reminder.Id,
                reminder.ResidentAlias,
                reminder.Category,
                reminder.Category.ToDisplayName(),
                reminder.CollectionPointName,
                $"Voce esta a {distanceKm:0.##} km de {reminder.CollectionPointName}. Hora de descartar {reminder.Category.ToDisplayName().ToLowerInvariant()} com seguranca.",
                distanceKm,
                now));
        }

        if (hasUpdates)
        {
            await _reminderRepository.SaveChangesAsync(cancellationToken);
        }

        return alerts
            .OrderBy(alert => alert.DistanceKm)
            .ToArray();
    }

    private static void ValidateCoordinates(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90)
        {
            throw new ArgumentException("Latitude invalida.", nameof(latitude));
        }

        if (longitude is < -180 or > 180)
        {
            throw new ArgumentException("Longitude invalida.", nameof(longitude));
        }
    }

    private static ReminderDto MapReminder(ReminderSubscription reminder) =>
        new(
            reminder.Id,
            reminder.ResidentAlias,
            reminder.Category,
            reminder.Category.ToDisplayName(),
            reminder.CollectionPointId,
            reminder.CollectionPointName,
            reminder.CollectionPointAddress,
            reminder.TargetLatitude,
            reminder.TargetLongitude,
            reminder.RadiusKm,
            reminder.BrowserNotificationsEnabled,
            reminder.CreatedAt,
            reminder.LastTriggeredAt);
}
