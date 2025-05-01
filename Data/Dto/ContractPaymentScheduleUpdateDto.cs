namespace Loans.Contracts.Data.Dto;

public class ContractPaymentScheduleUpdateDto : IContractUpdateDto
{
    public Guid ContractId { get; set; }
    public Guid PaymentScheduleId { get; set; }
}