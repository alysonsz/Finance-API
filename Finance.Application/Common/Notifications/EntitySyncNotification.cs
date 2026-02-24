using MediatR;

namespace Finance.Application.Common.Notifications;

public record EntitySyncNotification(
    string EntityType,
    string Operation,
    string Content) : INotification;