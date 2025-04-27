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
            .ForMember(dest => dest.ContractId, opt => opt.MapFrom(_ => Guid.NewGuid()));
    }
}