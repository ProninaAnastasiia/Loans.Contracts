using Loans.Contracts.Kafka.Events;

namespace Loans.Contracts.Handlers;

public interface ICreateContractRequestedHandler
{
    Task HandleAsync(CreateContractRequestedEvent contractEvent, CancellationToken cancellationToken);
}