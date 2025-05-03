namespace Loans.Contracts.Data.Dto;

public class ContractStatusUpdateDto : IContractUpdateDto
{
    public Guid ContractId { get; set; }
    public string ContractStatus { get; set; }
}