using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using Confluent.Kafka;
using StackExchange.Redis;
using Warehouse.Application.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace Warehouse.Infrastructure.Messaging;


public class StockEventConsumer : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<StockEventConsumer> _logger;

    public StockEventConsumer(string bootstrapServers,
        IConnectionMultiplexer redis, ILogger<StockEventConsumer> logger)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "warehouse-stock-cache-v2",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _redis = redis;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("StockEventConsumer запускается, подписываемся на stock-events");

        _consumer.Subscribe("stock-events");

        _logger.LogInformation("Подписка оформлена, начинаем читать сообщения");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(stoppingToken);
                _logger.LogInformation("Получено сообщение, offset {Offset}", result.Offset);

                await HandleMessageAsync(result);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось обработать сообщение из stock-events");

            }
        }
        _consumer.Close();
    }

    private async Task HandleMessageAsync(ConsumeResult<string, string> result)
    {
        
        var eventTypeHeader = result.Message.Headers.FirstOrDefault(h => h.Key == "event-type");
                
        if (eventTypeHeader is null)
        {
            _logger.LogWarning("Сообщение без заголовка event-type, пропускаем");
            return;
        }
        
        var eventType = Encoding.UTF8.GetString(eventTypeHeader.GetValueBytes());

        _logger.LogInformation("Тип события: {EventType}", eventType);

        Guid eventId;
        Guid itemId;
        Guid locationId;
        int delta;
        if (eventType == nameof(StockReceivedEvent))
        {
            var received = JsonSerializer.Deserialize<StockReceivedEvent>(result.Message.Value)!;
            eventId = received.EventId;
            itemId = received.ItemId;
            locationId = received.LocationId;
            delta = received.Quantity;
        }
        else if (eventType == nameof(StockWithdrawnEvent))
        {
            var received = JsonSerializer.Deserialize<StockWithdrawnEvent>(result.Message.Value)!;
            eventId = received.EventId;
            itemId = received.ItemId;
            locationId = received.LocationId;
            delta = received.Quantity;

        }
        else
        {
            _logger.LogWarning($"Неизвестный тип события {eventType}, пропускаем");
            return;
        }
        var db = _redis.GetDatabase();

        var dedupeKey = $"processed-events:{eventId}";
        var alreadyProcessed = await db.KeyExistsAsync(dedupeKey);


        if (alreadyProcessed)
        {
            _logger.LogInformation("Событие {EventId} уже обработано, пропускаем", eventId);
            return;
        }
        var stockKey = $"stock:{itemId}:{locationId}";
        await db.StringIncrementAsync(stockKey, delta);
        await db.StringSetAsync(dedupeKey, "1", TimeSpan.FromHours(24));

        _logger.LogInformation("Обновили остаток по ключу {StockKey} на {Delta}", stockKey, delta);

    }

}