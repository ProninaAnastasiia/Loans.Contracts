namespace Loans.Contracts.Kafka.Events.CalculateContractValues;

public record ContractScheduleUpdatedEvent(Guid ContractId, Guid ScheduleId, Guid OperationId) : EventBase;