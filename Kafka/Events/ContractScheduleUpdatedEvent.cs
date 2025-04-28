namespace Loans.Contracts.Kafka.Events;

public record ContractScheduleUpdatedEvent(Guid ContractId, Guid ScheduleId, Guid OperationId) : EventBase;