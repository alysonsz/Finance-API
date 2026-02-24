using Finance.Domain.Models;
using Finance.Infrastructure.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finance.Infrastructure.Repositories;

public abstract class BaseRepository<T>(FinanceWriteDbContext writeContext) where T : class
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = false
    };

    protected async Task CreateWithOutboxAsync(T entity)
    {
        await writeContext.Set<T>().AddAsync(entity);

        writeContext.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = typeof(T).Name,
            Operation = "INSERT",
            Content = JsonSerializer.Serialize(entity, _jsonOptions),
            CreatedAt = DateTime.UtcNow
        });

        await writeContext.SaveChangesAsync();
    }

    protected async Task UpdateWithOutboxAsync(T entity)
    {
        writeContext.Set<T>().Update(entity);

        writeContext.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = typeof(T).Name,
            Operation = "UPDATE",
            Content = JsonSerializer.Serialize(entity, _jsonOptions),
            CreatedAt = DateTime.UtcNow
        });

        await writeContext.SaveChangesAsync();
    }

    protected async Task DeleteWithOutboxAsync(T entity)
    {
        writeContext.Set<T>().Remove(entity);

        writeContext.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = typeof(T).Name,
            Operation = "DELETE",
            Content = JsonSerializer.Serialize(entity, _jsonOptions),
            CreatedAt = DateTime.UtcNow
        });

        await writeContext.SaveChangesAsync();
    }
}