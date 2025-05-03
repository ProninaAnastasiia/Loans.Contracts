using AutoMapper;
using Loans.Contracts.Data.Dto;
using Loans.Contracts.Data.Models;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Kafka.Events.CreateDraftContract;
using Loans.Contracts.Kafka.Events.GetContractApproved;

namespace Loans.Contracts.Data.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<LoanApplicationRequest, CreateContractRequestedEvent>();
        CreateMap<Contract, DraftContractCreatedEvent>();
        CreateMap<CreateContractRequestedEvent, Contract>()
            .ForMember(dest => dest.ContractId, opt => opt.MapFrom(_ => Guid.NewGuid()));
        CreateMap<ContractPaymentScheduleUpdateDto, Contract>()
            .ForMember(dest => dest.ContractId, opt => opt.Ignore());
        CreateMap<ContractFullLoanValueUpdateDto, Contract>()
            .ForMember(dest => dest.ContractId, opt => opt.Ignore());
        CreateMap<ContractValuesUpdateDto, Contract>()
            .ForMember(dest => dest.ContractId, opt => opt.Ignore());
        CreateMap<ContractStatusUpdateDto, Contract>()
            .ForMember(dest => dest.ContractId, opt => opt.Ignore());
        
        CreateMap<Contract, ContractDetailsResponseEvent>();

    }
}