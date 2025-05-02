using Loans.Contracts.Data.Dto;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Services;
using Newtonsoft.Json;

namespace Loans.Contracts.Kafka.Handlers;

public class ContractScheduleCalculatedHandler : IEventHandler<ContractScheduleCalculatedEvent>
{
    private readonly IContractService _сontractService;
    private readonly ILogger<ContractScheduleCalculatedHandler> _logger;
    private readonly IConfiguration _config;
    private KafkaProducerService _producer;
    
    public ContractScheduleCalculatedHandler(IContractService сontractService, ILogger<ContractScheduleCalculatedHandler> logger, IConfiguration config, KafkaProducerService producer)
    {
        _сontractService = сontractService;
        _logger = logger;
        _config = config;
        _producer = producer;
    }

    public async Task HandleAsync(ContractScheduleCalculatedEvent updateEvent, CancellationToken cancellationToken)
    {
        try
        {
            var contractUpdateDto = new ContractPaymentScheduleUpdateDto
            {
                ContractId = updateEvent.ContractId,
                PaymentScheduleId = updateEvent.ScheduleId
            };
            await _сontractService.UpdateContractAsync(contractUpdateDto, cancellationToken);
            
            var @event = new ContractScheduleUpdatedEvent(updateEvent.ContractId, updateEvent.ScheduleId,updateEvent.OperationId);
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var topic = _config["Kafka:Topics:UpdateContractRequested"];

            await _producer.PublishAsync(topic, jsonMessage);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to handle ContractScheduleCalculatedEvent. ContractId: {ContractId}, OperationId: {OperationId}. Exception: {e}", updateEvent.ContractId , updateEvent.OperationId, e.Message);
        }
    }
}