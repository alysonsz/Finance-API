using Finance.Application.Common.Notifications;
using Finance.Domain.Models;
using Finance.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Finance.Infrastructure.SyncHandler;

public class MainSyncHandler(FinanceReadDbContext readContext)
    : INotificationHandler<EntitySyncNotification>
{
    public async Task Handle(EntitySyncNotification notification, CancellationToken ct)
    {
        switch (notification.EntityType)
        {
            case nameof(Transaction):
                await SyncEntity<Transaction>(notification, readContext.Transactions, ct);
                break;
            case nameof(Category):
                await SyncEntity<Category>(notification, readContext.Categories, ct);
                break;
            case nameof(User):
                await SyncEntity<User>(notification, readContext.Users, ct);
                break;
        }
        await readContext.SaveChangesAsync(ct);
    }

    private async Task SyncEntity<T>(EntitySyncNotification note, DbSet<T> dbSet, CancellationToken ct) where T : class
    {
        var entity = JsonSerializer.Deserialize<T>(note.Content);

        if (entity == null) 
            return;

        var idProperty = typeof(T).GetProperty("Id");
        var idValue = idProperty?.GetValue(entity);

        if (note.Operation == "INSERT")
        {
            var exists = await dbSet.AnyAsync(e => EF.Property<object>(e, "Id") == idValue, ct);
            
            if (!exists)
                dbSet.Add(entity);
        }

        else if (note.Operation == "UPDATE")
            dbSet.Update(entity);

        else if (note.Operation == "DELETE")
            dbSet.Remove(entity);
    }
}