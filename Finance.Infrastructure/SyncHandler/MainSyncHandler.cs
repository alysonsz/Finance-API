using Finance.Application.Common.Notifications;
using Finance.Domain.Models;
using Finance.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Finance.Infrastructure.SyncHandler;

public class MainSyncHandler(FinanceReadDbContext readContext, Microsoft.Extensions.Logging.ILogger<MainSyncHandler> logger)
    : INotificationHandler<EntitySyncNotification>
{
    public async Task Handle(EntitySyncNotification notification, CancellationToken ct)
    {
        logger.LogInformation("[SYNC] Recebido {Operation} para {Type}", notification.Operation, notification.EntityType);
        try 
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
            logger.LogInformation("[SYNC] Sucesso ao sincronizar {Type}", notification.EntityType);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[SYNC] Falha crítica ao sincronizar {Type}", notification.EntityType);
        }
    }

    private async Task SyncEntity<T>(EntitySyncNotification note, DbSet<T> dbSet, CancellationToken ct) where T : class
    {
        var entity = JsonSerializer.Deserialize<T>(note.Content);

        if (entity == null) 
            return;

        if (entity is User user)
        {
            if (string.IsNullOrEmpty(user.PasswordHash))
                logger.LogWarning("[SYNC] ALERTA: Usuário {Email} está sendo sincronizado SEM SENHA!", user.Email);
            else
                logger.LogInformation("[SYNC] Senha do usuário {Email} recebida com sucesso ({Length} chars)", user.Email, user.PasswordHash.Length);
        }

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