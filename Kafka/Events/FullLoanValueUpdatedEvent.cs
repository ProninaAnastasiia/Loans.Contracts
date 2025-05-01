namespace Loans.Contracts.Kafka.Events;

public record FullLoanValueUpdatedEvent(Guid ContractId, decimal FullLoanValue, Guid OperationId) : EventBase;