namespace Loans.Contracts.Kafka.Handlers;

public interface IEventHandler<T>
{
    Task HandleAsync(T contractEvent, CancellationToken cancellationToken);
}