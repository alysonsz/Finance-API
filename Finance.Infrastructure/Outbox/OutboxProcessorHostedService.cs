using Finance.Application.Common.Notifications;
using Finance.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Finance.Infrastructure.Outbox;

public class OutboxProcessorHostedService(IServiceProvider serviceProvider, 
    ILogger<OutboxProcessorHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceWriteDbContext>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var messages = await dbContext.OutboxMessages
                .Where(m => m.ProcessedAt == null)
                .OrderBy(m => m.CreatedAt)
                .Take(20)
                .AsTracking()
                .ToListAsync(stoppingToken);

            foreach (var message in messages)
            {
                try
                {
                    await mediator.Publish(new EntitySyncNotification(
                        message.Type,
                        message.Operation,
                        message.Content), stoppingToken);

                    message.ProcessedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Falha ao processar mensagem {Id}", message.Id);
                    message.Error = ex.Message;
                }
            }

            if (messages.Any())
                await dbContext.SaveChangesAsync(stoppingToken);

            await Task.Delay(5000, stoppingToken);
        }
    }
}