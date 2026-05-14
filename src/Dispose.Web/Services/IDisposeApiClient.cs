using Dispose.Core.DTOs;
using Dispose.Core.Enums;

namespace Dispose.Web.Services;

public interface IDisposeApiClient
{
    Task<AssistantAvailabilityDto> GetAssistantStatusAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NeighborhoodSummaryDto>> GetNeighborhoodsAsync(CancellationToken cancellationToken = default);
    Task<DashboardOverviewDto> GetDashboardAsync(string neighborhoodName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CollectionScheduleDto>> GetSchedulesAsync(string? neighborhoodName, WasteType? wasteType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CollectionPointDto>> GetCollectionPointsAsync(
        string? neighborhoodName,
        SpecialItemCategory? category,
        double? latitude,
        double? longitude,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ReminderDto>> GetRemindersAsync(CancellationToken cancellationToken = default);
    Task<ReminderDto> CreateReminderAsync(CreateReminderRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReminderAlertDto>> CheckNearbyAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
    Task<AssistantResponseDto> AskAssistantAsync(AssistantRequestDto request, CancellationToken cancellationToken = default);
}
