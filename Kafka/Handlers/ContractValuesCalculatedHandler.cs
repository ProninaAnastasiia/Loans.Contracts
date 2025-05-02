using Loans.Contracts.Data.Dto;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Services;
using Newtonsoft.Json;

namespace Loans.Contracts.Kafka.Handlers;

public class ContractValuesCalculatedHandler : IEventHandler<ContractValuesCalculatedEvent>
{
    private readonly IContractService _сontractService;
    private readonly ILogger<ContractValuesCalculatedHandler> _logger;
    private readonly IConfiguration _config;
    private KafkaProducerService _producer;
    
    public ContractValuesCalculatedHandler(IContractService сontractService, ILogger<ContractValuesCalculatedHandler> logger, IConfiguration config, KafkaProducerService producer)
    {
        _сontractService = сontractService;
        _logger = logger;
        _config = config;
        _producer = producer;
    }

    public async Task HandleAsync(ContractValuesCalculatedEvent updateEvent, CancellationToken cancellationToken)
    {
        try
        {
            var contractUpdateDto = new ContractValuesUpdateDto
            {
                ContractId = updateEvent.ContractId,
                MonthlyPaymentAmount = updateEvent.MonthlyPaymentAmount,
                TotalPaymentAmount = updateEvent.TotalPaymentAmount,
                TotalInterestPaid = updateEvent.TotalInterestPaid,
                FullLoanValue = updateEvent.FullLoanValue
            };
            await _сontractService.UpdateContractAsync(contractUpdateDto, cancellationToken);
            
            var @event = new ContractValuesUpdatedEvent(updateEvent.ContractId, updateEvent.MonthlyPaymentAmount, 
                updateEvent.TotalPaymentAmount, updateEvent.TotalInterestPaid, updateEvent.FullLoanValue, updateEvent.OperationId);
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var topic = _config["Kafka:Topics:UpdateContractRequested"];

            await _producer.PublishAsync(topic, jsonMessage);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to handle ContractValuesCalculatedEvent. ContractId: {ContractId}, OperationId: {OperationId}. Exception: {e}", updateEvent.ContractId , updateEvent.OperationId, e.Message);
        }
    }
}