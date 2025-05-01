using System.Threading.Channels;
using Confluent.Kafka;
using Loans.Contracts.Kafka.Events;
using Loans.Contracts.Kafka.Handlers;
using Newtonsoft.Json.Linq;

namespace Loans.Contracts.Kafka.Consumers;

public class CreateContractConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CreateContractConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Channel<CreateContractRequestedEvent> _channel;
    private const int WorkerCount = 10;

    public CreateContractConsumer(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<CreateContractConsumer> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
        
        // Очередь на 1000 событий, дальше блокирует до освобождения места
        _channel = Channel.CreateBounded<CreateContractRequestedEvent>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
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
        consumer.Subscribe(_configuration["Kafka:Topics:CreateContractRequested"]);

        _logger.LogInformation("KafkaConsumerService CreateContractConsumer запущен.");
        
        // Стартуем воркеры
        for (int i = 0; i < WorkerCount; i++)
        {
            _ = Task.Run(() => ProcessEventsAsync(stoppingToken), stoppingToken);
        }

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);
                if (result == null) continue;

                _logger.LogInformation("Получено сообщение из Kafka: {Message}", result.Message.Value);
                
                var jsonObject = JObject.Parse(result.Message.Value);
                
                if (jsonObject.Property("EventType").Value.ToString().Contains("CreateContractRequestedEvent"))
                {
                    var @event = jsonObject.ToObject<CreateContractRequestedEvent>();
                    if (@event != null) await _channel.Writer.WriteAsync(@event, stoppingToken);
                }
            }
        }
        catch (KafkaException ex)
        {
            _logger.LogError(ex, "Kafka временно недоступна или ошибка получения сообщения.");
            await Task.Delay(1000, stoppingToken); // Ждем и пытаемся снова
        }
        catch (OperationCanceledException)
        {
            // Ничего не делаем — это ожидаемое поведение при остановке
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке события.");
        }
        finally
        {
            consumer.Close();
            _channel.Writer.Complete();
        }
    }
    
    private async Task ProcessEventsAsync(CancellationToken cancellationToken)
    {
        while (await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            while (_channel.Reader.TryRead(out var @event))
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
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
    }
}

