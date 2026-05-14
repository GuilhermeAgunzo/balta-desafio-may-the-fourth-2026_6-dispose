namespace Dispose.Web.Services;

public interface IReminderWatcher
{
    Task StartAsync();
    Task CheckNowAsync(CancellationToken cancellationToken = default);
}
