using AutoMapper;
using Loans.Contracts.Data.Dto;
using Loans.Contracts.Data.Models;
using Loans.Contracts.Kafka.Events;

namespace Loans.Contracts.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<LoanApplicationRequest, CreateContractRequestedEvent>();
        CreateMap<Contract, DraftContractCreatedEvent>();
        CreateMap<CreateContractRequestedEvent, Contract>()
            .ForMember(dest => dest.ContractId, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(src => Guid.Parse(src.ApplicationId)))
            .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => Guid.Parse(src.ClientId)))
            .ForMember(dest => dest.DecisionId, opt => opt.MapFrom(src => Guid.Parse(src.DecisionId)))
            .ForMember(dest => dest.CreditProductId, opt => opt.MapFrom(src => Guid.Parse(src.CreditProductId)))
            .ForMember(dest => dest.OperationId, opt => opt.MapFrom(src => Guid.Parse(src.OperationId)));
    }
}