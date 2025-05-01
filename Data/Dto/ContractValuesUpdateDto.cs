namespace Loans.Contracts.Data.Dto;

public class ContractValuesUpdateDto : IContractUpdateDto
{
    public Guid ContractId { get; set; }
    public decimal MonthlyPaymentAmount { get; set; }
    public decimal TotalPaymentAmount { get; set; }
    public decimal TotalInterestPaid { get; set; }
    public decimal FullLoanValue { get; set; }
}