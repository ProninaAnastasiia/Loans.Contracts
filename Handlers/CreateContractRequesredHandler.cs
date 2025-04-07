using Loans.Contracts.Kafka;
using Loans.Contracts.Kafka.Events;
using MediatR;

namespace Loans.Contracts.Handlers;

public class CreateContractRequesredHandler : INotificationHandler<CreateContractRequestedEvent>
{
    private readonly KafkaProducerService _kafkaProducerService;
    
    public CreateContractRequesredHandler(KafkaProducerService kafkaProducerService)
    {
        _kafkaProducerService = kafkaProducerService;
    }
    
    public async Task Handle(CreateContractRequestedEvent notification, CancellationToken cancellationToken)
    {
        //TODO: создать кредитный договор в бд
    }
}