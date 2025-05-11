using Confluent.Kafka;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Kafka.Events.CalculateContractValues;
using Loans.Contracts.Kafka.Events.GetContractApproved;
using Loans.Contracts.Kafka.Handlers;
using Newtonsoft.Json.Linq;

namespace Loans.Contracts.Kafka.Consumers;

public class UpdateContractConsumer : KafkaBackgroundConsumer
{
    public UpdateContractConsumer(
        IConfiguration config,
        IServiceProvider serviceProvider,
        ILogger<UpdateContractConsumer> logger)
        : base(config, serviceProvider, logger,
            topic: config["Kafka:Topics:UpdateContractRequested"],
            groupId: "contract-service-group",
            consumerName: nameof(UpdateContractConsumer)) { }

    protected override async Task HandleMessageAsync(JObject message, CancellationToken cancellationToken)
    {
        var eventType = message["EventType"]?.ToString();

        if (eventType?.Contains("ContractScheduleCalculatedEvent") == true)
        {
            var @event = message.ToObject<ContractScheduleCalculatedEvent>();
            if (@event != null) await ProcessContractScheduleCalculatedEventAsync(@event, cancellationToken);
        }
        else if (eventType?.Contains("ContractValuesCalculatedEvent") == true)
        {
            var @event = message.ToObject<ContractValuesCalculatedEvent>();
            if (@event != null) await ProcessContractValuesCalculatedEventAsync(@event, cancellationToken);
        }
        else if (eventType?.Contains("ContractDetailsRequestedEvent") == true)
        {
            var @event = message.ToObject<ContractDetailsRequestedEvent>();
            if (@event != null) await ProcessContractDetailsRequestedEventAsync(@event, cancellationToken);
        }
        else if (eventType?.Contains("UpdateContractStatusEvent") == true)
        {
            var @event = message.ToObject<UpdateContractStatusEvent>();
            if (@event != null) await ProcessUpdateContractStatusEventAsync(@event, cancellationToken);
        }
    }
    
    private async Task ProcessContractScheduleCalculatedEventAsync(ContractScheduleCalculatedEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<ContractScheduleCalculatedEvent>>();
            await handler.HandleAsync(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке события ContractScheduleCalculatedEvent: {EventId}, {OperationId}", @event.EventId, @event.OperationId);
            // Тут можно реализовать retry или логирование в dead-letter-topic
        }
    }
    
    private async Task ProcessContractValuesCalculatedEventAsync(ContractValuesCalculatedEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<ContractValuesCalculatedEvent>>();
            await handler.HandleAsync(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке события RepaymentScheduleCalculatedEvent: {EventId}, {OperationId}", @event.EventId, @event.OperationId);
            // Тут можно реализовать retry или логирование в dead-letter-topic
        }
    }
    
    private async Task ProcessContractDetailsRequestedEventAsync(ContractDetailsRequestedEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<ContractDetailsRequestedEvent>>();
            await handler.HandleAsync(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке события ContractDetailsRequestedEvent: {EventId}, {OperationId}", @event.EventId, @event.OperationId);
            // Тут можно реализовать retry или логирование в dead-letter-topic
        }
    }
    private async Task ProcessUpdateContractStatusEventAsync(UpdateContractStatusEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<UpdateContractStatusEvent>>();
            await handler.HandleAsync(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке события UpdateContractStatusEvent: {EventId}, {OperationId}", @event.EventId, @event.OperationId);
            // Тут можно реализовать retry или логирование в dead-letter-topic
        }
    }
    
}