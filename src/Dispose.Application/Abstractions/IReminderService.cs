using Dispose.Core.DTOs;

namespace Dispose.Application.Abstractions;

public interface IReminderService
{
    Task<IReadOnlyList<ReminderDto>> GetRemindersAsync(CancellationToken cancellationToken);
    Task<ReminderDto> CreateReminderAsync(CreateReminderRequestDto request, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReminderAlertDto>> CheckNearbyAsync(ProximityCheckRequestDto request, CancellationToken cancellationToken);
}
