namespace Loans.Contracts.Kafka.Events;

public abstract record EventBase : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public string EventType => GetType().AssemblyQualifiedName ?? nameof(EventBase);
}
