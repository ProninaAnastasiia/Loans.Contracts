﻿using Loans.Contracts.Data.Dto;
using Loans.Contracts.Data.Models;
using Loans.Contracts.Kafka.Events;

namespace Loans.Contracts.Services;

public interface IContractService
{
    Task<Contract> CreateContractAsync(CreateContractRequestedEvent request, CancellationToken cancellationToken);
    Task<Contract?> GetContractAsync(Guid contractId);
}