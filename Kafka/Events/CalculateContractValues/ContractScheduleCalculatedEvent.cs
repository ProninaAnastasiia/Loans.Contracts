namespace Loans.Contracts.Kafka.Events.CalculateContractValues;

public record ContractScheduleCalculatedEvent(Guid ContractId, Guid ScheduleId, Guid OperationId) : EventBase;