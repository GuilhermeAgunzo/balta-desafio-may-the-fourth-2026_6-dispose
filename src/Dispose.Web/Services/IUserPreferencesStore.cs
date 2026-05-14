using Dispose.Web.Models;

namespace Dispose.Web.Services;

public interface IUserPreferencesStore
{
    Task<UserPreferences> GetAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(UserPreferences preferences, CancellationToken cancellationToken = default);
}
