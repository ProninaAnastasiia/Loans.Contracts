using Loans.Contracts.Kafka.Events.CreateDraftContract;
using Loans.Contracts.Kafka.Handlers;
using Newtonsoft.Json.Linq;

namespace Loans.Contracts.Kafka.Consumers;

public class CreateContractConsumer : KafkaBackgroundConsumer
{
    public CreateContractConsumer(
        IConfiguration config,
        IServiceProvider serviceProvider,
        ILogger<CreateContractConsumer> logger)
        : base(config, serviceProvider, logger,
            topic: config["Kafka:Topics:CreateContractRequested"],
            groupId: "contract-service-group",
            consumerName: nameof(CreateContractConsumer)) { }
    
    protected override async Task HandleMessageAsync(JObject message, CancellationToken cancellationToken)
    {
        var eventType = message["EventType"]?.ToString();

        if (eventType?.Contains("CreateContractRequestedEvent") == true)
        {
            var @event = message.ToObject<CreateContractRequestedEvent>();
            if (@event != null) await ProcessCreateContractRequestedEventAsync(@event, cancellationToken);
        }
    }
    
    private async Task ProcessCreateContractRequestedEventAsync(CreateContractRequestedEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<CreateContractRequestedEvent>>();
            await handler.HandleAsync(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке события: {EventId}", @event.EventId);
            // Тут можно реализовать retry или логирование в dead-letter-topic
        }
    }
}

