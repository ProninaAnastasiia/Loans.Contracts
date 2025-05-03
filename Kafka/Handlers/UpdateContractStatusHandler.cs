using Loans.Contracts.Data.Dto;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Services;
using Newtonsoft.Json;

namespace Loans.Contracts.Kafka.Handlers;

public class UpdateContractStatusHandler : IEventHandler<UpdateContractStatusEvent>
{
    private readonly IContractService _сontractService;
    private readonly ILogger<UpdateContractStatusHandler> _logger;
    private readonly IConfiguration _config;
    private KafkaProducerService _producer;
    
    public UpdateContractStatusHandler(IContractService сontractService, ILogger<UpdateContractStatusHandler> logger, IConfiguration config, KafkaProducerService producer)
    {
        _сontractService = сontractService;
        _logger = logger;
        _config = config;
        _producer = producer;
    }

    public async Task HandleAsync(UpdateContractStatusEvent updateEvent, CancellationToken cancellationToken)
    {
        try
        {
            var contractUpdateDto = new ContractStatusUpdateDto
            {
                ContractId = updateEvent.ContractId,
                ContractStatus = updateEvent.Status
            };
            await _сontractService.UpdateContractAsync(contractUpdateDto, cancellationToken);
            
            var @event = new ContractStatusUpdatedEvent(updateEvent.ContractId, updateEvent.Status, updateEvent.OperationId);
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var topic = _config["Kafka:Topics:UpdateContractRequested"];

            await _producer.PublishAsync(topic, jsonMessage);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to handle UpdateContractStatusEvent. ContractId: {ContractId}, OperationId: {OperationId}. Exception: {e}", updateEvent.ContractId , updateEvent.OperationId, e.Message);
        }
    }
}