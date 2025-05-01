namespace Loans.Contracts.Data.Dto;

public class ContractFullLoanValueUpdateDto : IContractUpdateDto
{
    public Guid ContractId { get; set; }
    public decimal FullLoanValue { get; set; }
}