using AutoMapper;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Services;
using Newtonsoft.Json;

namespace Loans.Contracts.Kafka.Handlers;

public class GetFullContractHandler : IEventHandler<ContractDetailsRequestedEvent>
{
    private readonly IContractService _сontractService;
    private readonly ILogger<GetFullContractHandler> _logger;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    private KafkaProducerService _producer;
    
    public GetFullContractHandler(IMapper mapper, IContractService сontractService, ILogger<GetFullContractHandler> logger, IConfiguration config, KafkaProducerService producer)
    {
        _сontractService = сontractService;
        _logger = logger;
        _config = config;
        _producer = producer;
        _mapper = mapper;
    }

    public async Task HandleAsync(ContractDetailsRequestedEvent updateDetailsRequestedEvent, CancellationToken cancellationToken)
    {
        try
        {
            var contract = await _сontractService.GetContractAsync(updateDetailsRequestedEvent.ContractId);
            if (contract == null)
            {
                throw new Exception($"Contract {updateDetailsRequestedEvent.ContractId} not found");
            }
            
            var @event = _mapper.Map<ContractDetailsResponseEvent>(contract);
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var topic = _config["Kafka:Topics:CreateContractRequested"];

            await _producer.PublishAsync(topic, jsonMessage);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to handle ContractDetailsRequestedEvent. ContractId: {ContractId}, OperationId: {OperationId}. Exception: {e}", updateDetailsRequestedEvent.ContractId , updateDetailsRequestedEvent.OperationId, e.Message);
        }
    }
}