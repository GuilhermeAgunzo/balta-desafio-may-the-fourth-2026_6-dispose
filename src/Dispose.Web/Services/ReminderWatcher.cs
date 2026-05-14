using Dispose.Web.Models;

namespace Dispose.Web.Services;

public sealed class ReminderWatcher : IReminderWatcher, IAsyncDisposable
{
    private readonly IDisposeApiClient _apiClient;
    private readonly IUserPreferencesStore _preferencesStore;
    private readonly IBrowserBridge _browserBridge;
    private readonly AlertCenter _alertCenter;
    private readonly HashSet<string> _notifiedAlerts = [];
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _monitoringTask;

    public ReminderWatcher(
        IDisposeApiClient apiClient,
        IUserPreferencesStore preferencesStore,
        IBrowserBridge browserBridge,
        AlertCenter alertCenter)
    {
        _apiClient = apiClient;
        _preferencesStore = preferencesStore;
        _browserBridge = browserBridge;
        _alertCenter = alertCenter;
    }

    public Task StartAsync()
    {
        if (_monitoringTask is not null)
        {
            return Task.CompletedTask;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        _monitoringTask = MonitorAsync(_cancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    public async Task CheckNowAsync(CancellationToken cancellationToken = default)
    {
        var preferences = await _preferencesStore.GetAsync(cancellationToken);

        if (!preferences.LocationAlertsEnabled)
        {
            return;
        }

        var position = await _browserBridge.GetCurrentPositionAsync(cancellationToken);
        if (position is null)
        {
            return;
        }

        var alerts = await _apiClient.CheckNearbyAsync(position.Latitude, position.Longitude, cancellationToken);

        foreach (var alert in alerts)
        {
            var uniqueKey = $"{alert.ReminderId}:{alert.TriggeredAt:O}";
            if (!_notifiedAlerts.Add(uniqueKey))
            {
                continue;
            }

            _alertCenter.Publish("Alerta de proximidade", alert.AlertMessage, AlertTone.Success);

            if (preferences.BrowserNotificationsEnabled)
            {
                await _browserBridge.ShowNotificationAsync("Dispose", alert.AlertMessage, cancellationToken);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
        }
    }

    private async Task MonitorAsync(CancellationToken cancellationToken)
    {
        try
        {
            await CheckNowAsync(cancellationToken);
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(2));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await CheckNowAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore shutdown flow.
        }
        catch (Exception exception)
        {
            _alertCenter.Publish("Monitor de proximidade", exception.Message, AlertTone.Warning);
        }
    }
}
