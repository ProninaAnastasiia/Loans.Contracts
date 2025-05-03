namespace Loans.Contracts.Kafka.Events.CreateDraftContract;

public record CreateContractFailedEvent(Guid OperationId, string Error, string? InnerError) : EventBase;