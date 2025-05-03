namespace Loans.Contracts.Kafka.Events.CalculateContractValues;

public record ContractValuesUpdatedEvent(Guid ContractId, decimal MonthlyPaymentAmount,
    decimal TotalPaymentAmount,decimal TotalInterestPaid, decimal FullLoanValue, Guid OperationId) : EventBase;