using Dispose.Application.Abstractions;
using Dispose.Core.Entities;
using Dispose.Infra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Dispose.Infra.Repositories;

public sealed class ReminderRepository : IReminderRepository
{
    private readonly DisposeDbContext _dbContext;

    public ReminderRepository(DisposeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ReminderSubscription>> ListActiveAsync(CancellationToken cancellationToken)
    {
        var reminders = await _dbContext.ReminderSubscriptions.ToListAsync(cancellationToken);
        return reminders.OrderByDescending(reminder => reminder.CreatedAt).ToArray();
    }

    public async Task AddAsync(ReminderSubscription reminderSubscription, CancellationToken cancellationToken) =>
        await _dbContext.ReminderSubscriptions.AddAsync(reminderSubscription, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
