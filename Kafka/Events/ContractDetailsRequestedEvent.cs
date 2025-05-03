namespace Loans.Contracts.Kafka.Events;

public record ContractDetailsRequestedEvent(Guid ContractId, Guid OperationId) : EventBase;