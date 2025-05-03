using AutoMapper;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Kafka.Events.CreateDraftContract;
using Loans.Contracts.Services;
using Newtonsoft.Json;

namespace Loans.Contracts.Kafka.Handlers;

public class CreateContractRequestedHandler : IEventHandler<CreateContractRequestedEvent>
{
    private readonly IContractService _сontractService;
    private readonly ILogger<CreateContractRequestedHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private KafkaProducerService _producer;

    public CreateContractRequestedHandler(IContractService сontractService, ILogger<CreateContractRequestedHandler> logger,IMapper mapper,IConfiguration config, KafkaProducerService producer)
    {
        _сontractService = сontractService;
        _logger = logger;
        _mapper = mapper;
        _config = config;
        _producer = producer;
    }

    public async Task HandleAsync(CreateContractRequestedEvent request, CancellationToken cancellationToken)
    {
        var topic = _config["Kafka:Topics:CreateContractRequested"];
        try
        {
            var contract = await _сontractService.CreateContractAsync(request, cancellationToken);
            var @event = _mapper.Map<DraftContractCreatedEvent>(contract);
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _producer.PublishAsync(topic, jsonMessage);
            
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to handle CreateContractRequestedEvent. OperationId: {OperationId}. Exception: {e}", request.OperationId, e.Message);
            var @event = new CreateContractFailedEvent(request.OperationId, e.Message, e.InnerException?.Message);
            var jsonMessage = JsonConvert.SerializeObject(@event);
            await _producer.PublishAsync(topic, jsonMessage);
        }
    }
}

