using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dispose.Core.DTOs;
using Dispose.Core.Enums;

namespace Dispose.Web.Services;

public sealed class DisposeApiClient : IDisposeApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();
    private readonly HttpClient _httpClient;

    public DisposeApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<AssistantAvailabilityDto> GetAssistantStatusAsync(CancellationToken cancellationToken = default) =>
        GetAsync<AssistantAvailabilityDto>("api/assistant/status", cancellationToken);

    public Task<IReadOnlyList<NeighborhoodSummaryDto>> GetNeighborhoodsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<NeighborhoodSummaryDto>>("api/neighborhoods", cancellationToken);

    public Task<DashboardOverviewDto> GetDashboardAsync(string neighborhoodName, CancellationToken cancellationToken = default) =>
        GetAsync<DashboardOverviewDto>($"api/dashboard?neighborhood={Uri.EscapeDataString(neighborhoodName)}", cancellationToken);

    public Task<IReadOnlyList<CollectionScheduleDto>> GetSchedulesAsync(
        string? neighborhoodName,
        WasteType? wasteType,
        CancellationToken cancellationToken = default)
    {
        var segments = new List<string>();

        if (!string.IsNullOrWhiteSpace(neighborhoodName))
        {
            segments.Add($"neighborhood={Uri.EscapeDataString(neighborhoodName)}");
        }

        if (wasteType.HasValue)
        {
            segments.Add($"wasteType={wasteType}");
        }

        var query = segments.Count == 0 ? string.Empty : $"?{string.Join("&", segments)}";
        return GetAsync<IReadOnlyList<CollectionScheduleDto>>($"api/schedules{query}", cancellationToken);
    }

    public Task<IReadOnlyList<CollectionPointDto>> GetCollectionPointsAsync(
        string? neighborhoodName,
        SpecialItemCategory? category,
        double? latitude,
        double? longitude,
        CancellationToken cancellationToken = default)
    {
        var segments = new List<string>();

        if (!string.IsNullOrWhiteSpace(neighborhoodName))
        {
            segments.Add($"neighborhood={Uri.EscapeDataString(neighborhoodName)}");
        }

        if (category.HasValue)
        {
            segments.Add($"category={category}");
        }

        if (latitude.HasValue)
        {
            segments.Add($"latitude={latitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        }

        if (longitude.HasValue)
        {
            segments.Add($"longitude={longitude.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        }

        var query = segments.Count == 0 ? string.Empty : $"?{string.Join("&", segments)}";
        return GetAsync<IReadOnlyList<CollectionPointDto>>($"api/points{query}", cancellationToken);
    }

    public Task<IReadOnlyList<ReminderDto>> GetRemindersAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<ReminderDto>>("api/reminders", cancellationToken);

    public Task<ReminderDto> CreateReminderAsync(CreateReminderRequestDto request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateReminderRequestDto, ReminderDto>("api/reminders", request, cancellationToken);

    public Task<IReadOnlyList<ReminderAlertDto>> CheckNearbyAsync(double latitude, double longitude, CancellationToken cancellationToken = default) =>
        PostAsync<ProximityCheckRequestDto, IReadOnlyList<ReminderAlertDto>>(
            "api/reminders/check",
            new ProximityCheckRequestDto(latitude, longitude),
            cancellationToken);

    public Task<AssistantResponseDto> AskAssistantAsync(AssistantRequestDto request, CancellationToken cancellationToken = default) =>
        PostAsync<AssistantRequestDto, AssistantResponseDto>("api/assistant/query", request, cancellationToken);

    private async Task<TResponse> GetAsync<TResponse>(string uri, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(uri, cancellationToken);
        return await ReadContentAsync<TResponse>(response, cancellationToken);
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(string uri, TRequest request, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsJsonAsync(uri, request, JsonOptions, cancellationToken);
        return await ReadContentAsync<TResponse>(response, cancellationToken);
    }

    private static async Task<TResponse> ReadContentAsync<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(error) ? "Falha ao consultar a API." : error);
        }

        var payload = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
        return payload ?? throw new InvalidOperationException("A API retornou uma resposta vazia.");
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
