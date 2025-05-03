namespace Loans.Contracts.Kafka.Events;

public record UpdateContractStatusEvent(Guid ContractId, string Status, Guid OperationId) : EventBase;