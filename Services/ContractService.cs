using Loans.Contracts.Data;
using Loans.Contracts.Data.Dto;
using Loans.Contracts.Data.Models;
using Loans.Contracts.Kafka;
using Loans.Contracts.Kafka.Events;

namespace Loans.Contracts.Services;

public class ContractService : IContractService
{
    private readonly LoanContractContext _dbContext;

    public ContractService(LoanContractContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Contract> CreateContractAsync(CreateContractRequestedEvent loanApplicationRequest, Guid operationId)
    {
        // Генерация уникального ID для контракта
        var contractId = Guid.NewGuid();

        // Создаем черновик контракта
        var contract = new Contract();

        // Добавляем залог
        if (loanApplicationRequest.Pawn != null)
        {
            contract.Pawn = new Pawn
            {
                Type = loanApplicationRequest.Pawn.Type,
                Description = loanApplicationRequest.Pawn.Description,
                PawnAmount = loanApplicationRequest.Pawn.PawnAmount
            };
        }

        // Добавляем страховку
        if (loanApplicationRequest.Insurance != null)
        {
            contract.Insurance = new Insurance
            {
                Type = loanApplicationRequest.Insurance.Type,
                Amount = loanApplicationRequest.Insurance.Amount,
                Company = loanApplicationRequest.Insurance.Company
            };
        }

        // Сохраняем контракт в базе данных
        _dbContext.Contracts.Add(contract);
        await _dbContext.SaveChangesAsync();

        // Публикуем событие с помощью MediatR
        //TODO: создать событие кредитный договор создан и хэндлер к нему 

        return contract;
    }

    public async Task<Contract?> GetContractAsync(Guid contractId)
    {
        return (await _dbContext.Contracts.FindAsync(contractId));
    }
}