using System.Text.Json;
using Dispose.Web.Models;
using Microsoft.JSInterop;

namespace Dispose.Web.Services;

public sealed class BrowserPreferencesStore : IUserPreferencesStore
{
    private const string StorageKey = "dispose.preferences";
    private readonly IJSRuntime _jsRuntime;

    public BrowserPreferencesStore(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<UserPreferences> GetAsync(CancellationToken cancellationToken = default)
    {
        var serializedPreferences = await _jsRuntime.InvokeAsync<string?>("disposeBrowser.getItem", cancellationToken, StorageKey);

        if (string.IsNullOrWhiteSpace(serializedPreferences))
        {
            return UserPreferences.Default;
        }

        return JsonSerializer.Deserialize<UserPreferences>(serializedPreferences) ?? UserPreferences.Default;
    }

    public async Task SaveAsync(UserPreferences preferences, CancellationToken cancellationToken = default)
    {
        var serializedPreferences = JsonSerializer.Serialize(preferences);
        await _jsRuntime.InvokeAsync<bool>("disposeBrowser.setItem", cancellationToken, StorageKey, serializedPreferences);
    }
}
