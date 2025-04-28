using AutoMapper;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Services;
using Newtonsoft.Json;

namespace Loans.Contracts.Kafka.Handlers;

public class RepaymentScheduleCalculatedHandler: IEventHandler<RepaymentScheduleCalculatedEvent>
{
    private readonly IContractService _сontractService;
    private readonly ILogger<RepaymentScheduleCalculatedHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private KafkaProducerService _producer;
    
    public RepaymentScheduleCalculatedHandler(IContractService сontractService, ILogger<RepaymentScheduleCalculatedHandler> logger, IMapper mapper,IConfiguration config, KafkaProducerService producer)
    {
        _сontractService = сontractService;
        _logger = logger;
        _mapper = mapper;
        _config = config;
        _producer = producer;
    }

    public async Task HandleAsync(RepaymentScheduleCalculatedEvent contractEvent, CancellationToken cancellationToken)
    {
        try
        {
            await _сontractService.UpdateContractScheduleAsync(contractEvent, cancellationToken);
            var @event = new ContractScheduleUpdatedEvent(contractEvent.ContractId, contractEvent.ScheduleId, contractEvent.OperationId);
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var topic = _config["Kafka:Topics:UpdateContractRequested"];

            await _producer.PublishAsync(topic, jsonMessage);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to handle ContractScheduleUpdatedEvent. OperationId: {OperationId}. Exception: {e}", contractEvent.OperationId, e.Message);
        }
    }
}