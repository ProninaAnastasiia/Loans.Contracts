using Loans.Contracts.Data.Models;

namespace Loans.Contracts.Kafka.Events;

public record CreateContractRequestedEvent(
    string ApplicationId, string ClientId, string DecisionId, DateTime LodgementDate,
    string CreditProductId, decimal LoanAmount, int LoanTermMonths,
    decimal InterestRate, string LoanPurpose, string LoanType, string PaymentType,
    decimal? InitialPaymentAmount, Pawn? Pawn, Insurance? Insurance) : IEvent;