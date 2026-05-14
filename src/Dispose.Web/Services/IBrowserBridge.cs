using Dispose.Web.Models;

namespace Dispose.Web.Services;

public interface IBrowserBridge
{
    Task<GeoPosition?> GetCurrentPositionAsync(CancellationToken cancellationToken = default);
    Task<string> RequestNotificationPermissionAsync(CancellationToken cancellationToken = default);
    Task<bool> ShowNotificationAsync(string title, string body, CancellationToken cancellationToken = default);
}
