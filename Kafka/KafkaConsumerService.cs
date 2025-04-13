using Confluent.Kafka;
using Loans.Contracts.Handlers;
using Loans.Contracts.Kafka.Events;
using Newtonsoft.Json;

namespace Loans.Contracts.Kafka;

public class KafkaConsumerService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public KafkaConsumerService(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<KafkaConsumerService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = "contract-service-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            consumer.Subscribe(_configuration["Kafka:Topics:CreateContractRequested"]);

            _logger.LogInformation("KafkaConsumerService запущен.");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (result == null)
                    {
                        _logger.LogDebug("Нет новых сообщений из Kafka.");
                        continue;
                    }

                    _logger.LogInformation("Получено сообщение из Kafka: {Message}", result.Message.Value);

                    var @event = JsonConvert.DeserializeObject<CreateContractRequestedEvent>(result.Message.Value);

                    if (@event != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var handler = scope.ServiceProvider.GetRequiredService<ICreateContractRequestedHandler>();
                        handler.HandleAsync(@event, stoppingToken).GetAwaiter().GetResult(); // sync-over-async, допустимо тут
                    }
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Ошибка при чтении из Kafka.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке события.");
            }
            finally
            {
                consumer.Close();
            }
        }, stoppingToken);
    }

}
