namespace Loans.Contracts.Kafka.Events;

public record ContractStatusUpdatedEvent(Guid ContractId, string Status, Guid OperationId) : EventBase;