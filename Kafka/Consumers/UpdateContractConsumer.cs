using Confluent.Kafka;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Kafka.Handlers;
using Newtonsoft.Json.Linq;

namespace Loans.Contracts.Kafka.Consumers;

public class UpdateContractConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<UpdateContractConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public UpdateContractConsumer(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<UpdateContractConsumer> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(3000, stoppingToken); // дать приложению прогрузиться
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "contract-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(_configuration["Kafka:Topics:UpdateContractRequested"]);

        _logger.LogInformation("KafkaConsumerService UpdateContractConsumer запущен.");
        
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);
                if (result == null) continue;

                var jsonObject = JObject.Parse(result.Message.Value);

                if (jsonObject.Property("EventType").Value.ToString().Contains("ContractScheduleCalculatedEvent"))
                {
                    _logger.LogInformation("Получено сообщение из Kafka: {Message}", result.Message.Value);
                    var @event = jsonObject.ToObject<ContractScheduleCalculatedEvent>();
                    if (@event != null) await ProcessContractScheduleCalculatedEventAsync(@event, stoppingToken);
                }
                if (jsonObject.Property("EventType").Value.ToString().Contains("ContractValuesCalculatedEvent"))
                {
                    _logger.LogInformation("Получено сообщение из Kafka: {Message}", result.Message.Value);
                    var @event = jsonObject.ToObject<ContractValuesCalculatedEvent>();
                    if (@event != null) await ProcessContractValuesCalculatedEventAsync(@event, stoppingToken);
                }
                if (jsonObject.Property("EventType").Value.ToString().Contains("ContractDetailsRequestedEvent"))
                {
                    _logger.LogInformation("Получено сообщение из Kafka: {Message}", result.Message.Value);
                    var @event = jsonObject.ToObject<ContractDetailsRequestedEvent>();
                    if (@event != null) await ProcessGetFullContractEventAsync(@event, stoppingToken);
                }
                if (jsonObject.Property("EventType").Value.ToString().Contains("UpdateContractStatusEvent"))
                {
                    _logger.LogInformation("Получено сообщение из Kafka: {Message}", result.Message.Value);
                    var @event = jsonObject.ToObject<UpdateContractStatusEvent>();
                    if (@event != null) await ProcessUpdateContractStatusEventAsync(@event, stoppingToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Нормальное завершение — ничего не логируем
        }
        catch (KafkaException ex)
        {
            _logger.LogError(ex, "Kafka временно недоступна или ошибка получения сообщения.");
            await Task.Delay(1000, stoppingToken); // Ждем и пытаемся снова
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке события.");
        }
        finally
        {
            consumer.Close();
        }
    }
    
    private async Task ProcessContractScheduleCalculatedEventAsync(ContractScheduleCalculatedEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
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
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<ContractValuesCalculatedEvent>>();
            await handler.HandleAsync(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке события RepaymentScheduleCalculatedEvent: {EventId}, {OperationId}", @event.EventId, @event.OperationId);
            // Тут можно реализовать retry или логирование в dead-letter-topic
        }
    }
    
    private async Task ProcessGetFullContractEventAsync(ContractDetailsRequestedEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
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
            using var scope = _serviceProvider.CreateScope();
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