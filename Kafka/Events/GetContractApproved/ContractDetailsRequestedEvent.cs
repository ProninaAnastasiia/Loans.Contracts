namespace Loans.Contracts.Kafka.Events.GetContractApproved;

public record ContractDetailsRequestedEvent(Guid ContractId, Guid OperationId) : EventBase;