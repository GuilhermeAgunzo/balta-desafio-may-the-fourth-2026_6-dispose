using Dispose.Web.Models;

namespace Dispose.Web.Services;

public sealed class AlertCenter
{
    private readonly List<UiAlert> _alerts = [];

    public event Action? Changed;

    public IReadOnlyList<UiAlert> Alerts => _alerts;

    public void Publish(string title, string message, AlertTone tone)
    {
        _alerts.Insert(0, new UiAlert(Guid.NewGuid(), title, message, tone));

        if (_alerts.Count > 4)
        {
            _alerts.RemoveAt(_alerts.Count - 1);
        }

        Changed?.Invoke();
    }

    public void Dismiss(Guid alertId)
    {
        if (_alerts.RemoveAll(alert => alert.Id == alertId) > 0)
        {
            Changed?.Invoke();
        }
    }
}
