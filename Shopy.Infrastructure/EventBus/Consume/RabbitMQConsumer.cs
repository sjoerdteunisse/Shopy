using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shopy.Core.Events;
using Shopy.Infrastructure.EventBus.Produce;

namespace Shopy.Infrastructure.EventBus.Consume;

public class RabbitMQConsumer(
    RabbitMQConfiguration config,
    IEventBusLogger logger) : IRabbitMQConsumer, IAsyncDisposable
{
    private IConnection? _connection;
    private readonly EventTypeRegistry _typeRegistry = new();

    /// <summary>
    /// Initialize consumer asynchronously
    /// </summary>
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = config.HostName,
                Port = config.Port,
                UserName = config.Username,
                Password = config.Password,
            };
            
            _connection = await factory.CreateConnectionAsync(cancellationToken);
            
            logger.LogSubscribing("RabbitMQConsumer", $"Connected to {config.HostName}:{config.Port}");
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to initialize consumer", ex);
            throw;
        }
    }
    
    /// <summary>
    /// Listens to queue and processes messages
    /// </summary>
    public async Task SubscribeAsync(
        string serviceName,
        string queueName,
        Func<IDomainEvent, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default)
    {
        if (_connection == null)
            throw new InvalidOperationException("Consumer not initialized. Call InitializeAsync first.");
        
        var channel = await _connection.CreateChannelAsync(
            cancellationToken: cancellationToken);
        
        try
        {
            await channel.ExchangeDeclareAsync(
                exchange: config.ExchangeName,
                type: config.ExchangeType,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);
            
            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);
            
            await channel.QueueBindAsync(
                queue: queueName,
                exchange: config.ExchangeName,
                routingKey: string.Empty,
                arguments: null,
                cancellationToken: cancellationToken);
            
            await channel.BasicQosAsync(0, config.PrefetchCount, false, cancellationToken);
            
            var consumer = new AsyncEventingBasicConsumer(channel);
            
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var deliveryTag = ea.DeliveryTag;
                
                try
                {
                    var body = ea.Body.ToArray();
                    var eventJson = System.Text.Encoding.UTF8.GetString(body);
                    
                    var eventTypeName = ea.BasicProperties?.Type ?? "Unknown";
                    var eventType = _typeRegistry.GetEventType(eventTypeName);
                    
                    if (eventType == null)
                    {
                        logger.LogError($"Unknown event type: {eventTypeName}");
                        await channel.BasicNackAsync(deliveryTag, false, false, cancellationToken);
                        return;
                    }
                    
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var @event = (IDomainEvent?)JsonSerializer.Deserialize(eventJson, eventType, options);
                    
                    if (@event == null)
                    {
                        logger.LogError($"Failed to deserialize {eventTypeName}");
                        await channel.BasicNackAsync(deliveryTag, false, false, cancellationToken);
                        return;
                    }
                    
                    logger.LogMessageReceived(@event.EventType, serviceName);
                    
                    await handler(@event, cancellationToken);
                    await channel.BasicAckAsync(deliveryTag, false, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error processing message: {ex.Message}", ex);
                    await channel.BasicNackAsync(deliveryTag, false, true, cancellationToken);
                }
            };
            
            string consumerTag = await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumerTag: $"{queueName}-consumer",
                noLocal: false,
                exclusive: false,
                arguments: null,
                consumer: consumer,
                cancellationToken: cancellationToken);
            
            logger.LogSubscribing(serviceName, queueName);
            
            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                await channel.BasicCancelAsync(consumerTag, true, cancellationToken);
            }
        }
        finally
        {
            await channel.CloseAsync(cancellationToken: cancellationToken);
            await channel.DisposeAsync();
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
            await _connection.CloseAsync();
        _connection?.Dispose();
    }
}