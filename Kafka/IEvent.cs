using MediatR;

namespace Loans.Contracts.Kafka;

public interface IEvent: INotification
{
    Guid EventId => Guid.NewGuid();
    public DateTime OccurredOn => DateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName;
}