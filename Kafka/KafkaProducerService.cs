using Confluent.Kafka;

namespace Loans.Contracts.Kafka;

public class KafkaProducerService
{
    private readonly IConfiguration _configuration;
    private readonly IProducer<Null, string> _producer;

    public KafkaProducerService(IConfiguration configuration)
    {
        _configuration = configuration;

        var producerconfig = new ProducerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<Null, string>(producerconfig).Build();
    }

    public async Task PublishAsync(string topic, string message)
    {
        var kafkamessage = new Message<Null, string> { Value = message, };

        await _producer.ProduceAsync(topic, kafkamessage);
    }
}