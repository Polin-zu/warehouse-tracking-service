using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using Confluent.Kafka;
using Warehouse.Application.Abstractions;

namespace Warehouse.Infrastructure.Messaging;

public class KafkaEventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly IProducer<string, string> _producer;

    public KafkaEventPublisher(string bootstrapServers)
    {
        var config = new ProducerConfig { BootstrapServers = bootstrapServers };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }
    public async Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(@event);
        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = json
        };
        await _producer.ProduceAsync(topic, message, cancellationToken);
    }
    public async ValueTask DisposeAsync()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
        await Task.CompletedTask;
    }
}
