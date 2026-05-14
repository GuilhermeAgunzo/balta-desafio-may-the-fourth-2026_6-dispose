using Dispose.Web.Models;
using Microsoft.JSInterop;

namespace Dispose.Web.Services;

public sealed class BrowserBridge : IBrowserBridge
{
    private readonly IJSRuntime _jsRuntime;

    public BrowserBridge(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public Task<GeoPosition?> GetCurrentPositionAsync(CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<GeoPosition?>("disposeBrowser.getCurrentPosition", cancellationToken).AsTask();

    public async Task<string> RequestNotificationPermissionAsync(CancellationToken cancellationToken = default) =>
        await _jsRuntime.InvokeAsync<string>("disposeBrowser.requestNotificationPermission", cancellationToken);

    public async Task<bool> ShowNotificationAsync(string title, string body, CancellationToken cancellationToken = default) =>
        await _jsRuntime.InvokeAsync<bool>("disposeBrowser.showNotification", cancellationToken, title, body);
}
