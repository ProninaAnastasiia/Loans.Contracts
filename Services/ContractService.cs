using AutoMapper;
using Loans.Contracts.Data;
using Loans.Contracts.Data.Models;
using Loans.Contracts.Kafka.Events;
using Microsoft.EntityFrameworkCore;

namespace Loans.Contracts.Services;

public class ContractService : IContractService
{
    private readonly LoanContractDbContext _dbContext;
    private readonly ILogger<ContractService> _logger;
    private readonly IMapper _mapper;

    public ContractService(LoanContractDbContext dbContext, ILogger<ContractService> logger, IMapper mapper)
    {
        _dbContext = dbContext;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Contract> CreateContractAsync(CreateContractRequestedEvent request, CancellationToken cancellationToken)
    {
        try
        {
            var newContract = _mapper.Map<Contract>(request);
            var existingContract = await _dbContext.Contracts.FirstOrDefaultAsync(u => u.OperationId.Equals(newContract.OperationId));
            
            if (existingContract == null)
            {
                newContract.ContractStatus = "Черновик";
                
                if (newContract.LoanType.Equals("Ипотека") || newContract.LoanType.Equals("Автокредит"))
                {
                    var pawn = new Pawn
                    {
                        PawnId = Guid.NewGuid(),
                        PawnAmount = newContract.LoanAmount
                    };

                    switch (newContract.LoanType)
                    {
                        case "Ипотека":
                            pawn.Type = "Недвижимость";
                            pawn.Description = "Квартира/Дом";
                            break;
                        case "Автокредит":
                            pawn.Type = "Транспортное средство";
                            pawn.Description = "Автомобиль";
                            break;
                        default:
                            pawn.Type = "Не определено";
                            pawn.Description = "Тип кредита не поддерживается для залога";
                            _logger.LogWarning("Неподдерживаемый тип кредита для залога: {LoanType}", newContract.LoanType);
                            break;
                    }
                    _dbContext.Pawns.Add(pawn); 
                    newContract.Pawn = pawn;
                }

                var insurance = new Insurance
                {
                    InsuranceId = Guid.NewGuid(),
                    Description = "Добровольная страховка",
                    Amount = newContract.LoanAmount * (decimal)0.2,
                    Company = "ООО Согаз"
                };
                _dbContext.Insurances.Add(insurance);
                
                newContract.Insurance = insurance;
                newContract.AdditionalTerms = "Дополнительные условия: Досрочное погашение разрешено на сумму не менее половины от запланированного ежемесячного платежа.";
                newContract.InitialPaymentAmount = newContract.LoanAmount*(decimal)0.15;
                newContract.LastModificationDate = DateTime.UtcNow;
                
                _dbContext.Contracts.Add(newContract);
                await _dbContext.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("Контракт создан: {ContractId}", newContract.ContractId);
            }
            else
            {
                _logger.LogWarning("Повторная попытка создать контракт по OperationId: {OperationId}", existingContract.OperationId);
                return existingContract;
            }
            return newContract;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании договора");
            throw;
        }
    }

    public async Task<Contract?> GetContractAsync(Guid contractId)
    {
        return (await _dbContext.Contracts.FindAsync(contractId));
    }
}