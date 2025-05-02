namespace Loans.Contracts.Kafka.Events;

public record ContractScheduleCalculatedEvent(Guid ContractId, Guid ScheduleId, Guid OperationId) : EventBase;