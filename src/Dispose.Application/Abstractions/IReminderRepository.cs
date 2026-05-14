using Dispose.Core.Entities;

namespace Dispose.Application.Abstractions;

public interface IReminderRepository
{
    Task<IReadOnlyList<ReminderSubscription>> ListActiveAsync(CancellationToken cancellationToken);
    Task AddAsync(ReminderSubscription reminderSubscription, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
