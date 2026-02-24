using MediatR;

namespace Finance.Application.Common.Notifications;

public record EntityChangedNotification<T>(T Entity, string Operation) : INotification where T : class;