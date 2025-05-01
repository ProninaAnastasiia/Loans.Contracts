using Loans.Contracts.Data.Dto;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Services;
using Newtonsoft.Json;

namespace Loans.Contracts.Kafka.Handlers;

public class RepaymentScheduleCalculatedHandler : IEventHandler<RepaymentScheduleCalculatedEvent>
{
    private readonly IContractService _сontractService;
    private readonly ILogger<RepaymentScheduleCalculatedHandler> _logger;
    private readonly IConfiguration _config;
    private KafkaProducerService _producer;
    
    public RepaymentScheduleCalculatedHandler(IContractService сontractService, ILogger<RepaymentScheduleCalculatedHandler> logger, IConfiguration config, KafkaProducerService producer)
    {
        _сontractService = сontractService;
        _logger = logger;
        _config = config;
        _producer = producer;
    }

    public async Task HandleAsync(RepaymentScheduleCalculatedEvent updateEvent, CancellationToken cancellationToken)
    {
        try
        {
            //TODO: Свести до одного хэндлера общего события и менять договор одним запросом к БД
            //TODO: И после генерировать событие типа Контракт готов к подписанию
            var contractUpdateDto = new ContractPaymentScheduleUpdateDto
            {
                ContractId = updateEvent.ContractId,
                PaymentScheduleId = updateEvent.ScheduleId
            };
            await _сontractService.UpdateContractAsync(contractUpdateDto, cancellationToken);
            
            var @event = new ContractScheduleUpdatedEvent(updateEvent.ContractId, updateEvent.ScheduleId, updateEvent.OperationId);
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var topic = _config["Kafka:Topics:UpdateContractRequested"];

            await _producer.PublishAsync(topic, jsonMessage);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to handle RepaymentScheduleCalculatedEvent. OperationId: {OperationId}. Exception: {e}", updateEvent.OperationId, e.Message);
        }
    }
}