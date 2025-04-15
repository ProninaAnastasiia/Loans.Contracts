namespace Loans.Contracts.Kafka.Events;

public record CreateContractFailedEvent(string OperationId, string Error) : EventBase;