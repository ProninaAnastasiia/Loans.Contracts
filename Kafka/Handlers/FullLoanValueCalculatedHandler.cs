using Loans.Contracts.Data.Dto;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Services;
using Newtonsoft.Json;

namespace Loans.Contracts.Kafka.Handlers;

public class FullLoanValueCalculatedHandler : IEventHandler<FullLoanValueCalculatedEvent>
{
    private readonly IContractService _сontractService;
    private readonly ILogger<FullLoanValueCalculatedHandler> _logger;
    private readonly IConfiguration _config;
    private KafkaProducerService _producer;
    
    public FullLoanValueCalculatedHandler(IContractService сontractService, ILogger<FullLoanValueCalculatedHandler> logger, IConfiguration config, KafkaProducerService producer)
    {
        _сontractService = сontractService;
        _logger = logger;
        _config = config;
        _producer = producer;
    }

    public async Task HandleAsync(FullLoanValueCalculatedEvent updateEvent, CancellationToken cancellationToken)
    {
        try
        {
            var contractUpdateDto = new ContractFullLoanValueUpdateDto
            {
                ContractId = updateEvent.ContractId,
                FullLoanValue = updateEvent.FullLoanValue
            };
            await _сontractService.UpdateContractAsync(contractUpdateDto, cancellationToken);
            var @event = new FullLoanValueUpdatedEvent(updateEvent.ContractId, updateEvent.FullLoanValue, updateEvent.OperationId);
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var topic = _config["Kafka:Topics:UpdateContractRequested"];

            await _producer.PublishAsync(topic, jsonMessage);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to handle ContractScheduleUpdatedEvent. OperationId: {OperationId}. Exception: {e}", updateEvent.OperationId, e.Message);
        }
    }
}