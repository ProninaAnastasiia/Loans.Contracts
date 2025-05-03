using Loans.Contracts.Data.Dto;
using Loans.Contracts.Data.Models;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Kafka.Events.CreateDraftContract;

namespace Loans.Contracts.Services;

public interface IContractService
{
    Task<Contract> CreateContractAsync(CreateContractRequestedEvent request, CancellationToken cancellationToken);
    Task UpdateContractAsync(IContractUpdateDto contractUpdateDto, CancellationToken cancellationToken);
    Task<Contract?> GetContractAsync(Guid contractId);
}