using Dispose.Core.DTOs;
using Dispose.Core.Enums;
using Dispose.Web.Models;
using Dispose.Web.Services;

namespace Dispose.Web.Tests;

internal sealed class FakeDisposeApiClient : IDisposeApiClient
{
    public bool AssistantConfigured { get; set; } = true;

    public Task<AssistantAvailabilityDto> GetAssistantStatusAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(new AssistantAvailabilityDto(AssistantConfigured, "OpenAI", "gpt-4.1-mini"));

    public Task<IReadOnlyList<NeighborhoodSummaryDto>> GetNeighborhoodsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<NeighborhoodSummaryDto>>(
        [
            new NeighborhoodSummaryDto(Guid.NewGuid(), "Coruscant Centro", "Setor Senado", "Teste", -23.55, -46.63)
        ]);

    public Task<DashboardOverviewDto> GetDashboardAsync(string neighborhoodName, CancellationToken cancellationToken = default) =>
        Task.FromResult(
            new DashboardOverviewDto(
                neighborhoodName,
                "Setor Senado",
                "Teste",
                [
                    new UpcomingCollectionDto(Guid.NewGuid(), WasteType.Glass, "Vidro", DayOfWeek.Friday, "Sexta", new DateOnly(2026, 5, 15), "08:00-10:00", "ROT-1", "Embale o vidro")
                ],
                [
                    new CollectionPointDto(Guid.NewGuid(), "Hangar Verde", neighborhoodName, "Rua 1", "Base", "Alianca", -23.55, -46.63, [SpecialItemCategory.Batteries], ["Pilhas e baterias"], 0.4)
                ],
                2,
                DateTimeOffset.UtcNow));

    public Task<IReadOnlyList<CollectionScheduleDto>> GetSchedulesAsync(string? neighborhoodName, WasteType? wasteType, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CollectionScheduleDto>>(Array.Empty<CollectionScheduleDto>());

    public Task<IReadOnlyList<CollectionPointDto>> GetCollectionPointsAsync(string? neighborhoodName, SpecialItemCategory? category, double? latitude, double? longitude, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CollectionPointDto>>(Array.Empty<CollectionPointDto>());

    public Task<IReadOnlyList<ReminderDto>> GetRemindersAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<ReminderDto>>(Array.Empty<ReminderDto>());

    public Task<ReminderDto> CreateReminderAsync(CreateReminderRequestDto request, CancellationToken cancellationToken = default) =>
        Task.FromResult(new ReminderDto(Guid.NewGuid(), request.ResidentAlias, request.Category, request.Category.ToString(), request.CollectionPointId, "Ponto", "Endereco", -23.55, -46.63, request.RadiusKm, request.BrowserNotificationsEnabled, DateTimeOffset.UtcNow, null));

    public Task<IReadOnlyList<ReminderAlertDto>> CheckNearbyAsync(double latitude, double longitude, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<ReminderAlertDto>>(Array.Empty<ReminderAlertDto>());

    public Task<AssistantResponseDto> AskAssistantAsync(AssistantRequestDto request, CancellationToken cancellationToken = default) =>
        Task.FromResult(new AssistantResponseDto(request.Question, "Resposta simulada", request.NeighborhoodName, ["Abrir tela"], DateTimeOffset.UtcNow));
}

internal sealed class FakePreferencesStore : IUserPreferencesStore
{
    public Task<UserPreferences> GetAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(new UserPreferences
        {
            PreferredNeighborhood = "Coruscant Centro",
            PilotCallsign = "Tripulante Rebelde",
            ReminderRadiusKm = 0.7,
            BrowserNotificationsEnabled = true,
            LocationAlertsEnabled = true
        });

    public Task SaveAsync(UserPreferences preferences, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
