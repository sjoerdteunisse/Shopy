using System.Text.Json;
using RabbitMQ.Client;
using Shopy.Core.Events;

namespace Shopy.Infrastructure.EventBus.Produce;

public class RabbitMQPublisher(
    RabbitMQConfiguration config,
    IEventBusLogger logger,
    string serviceName)

    : IRabbitMQPublisher, IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

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
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.ExchangeDeclareAsync(
                exchange: config.ExchangeName,
                type: config.ExchangeType,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);
            
            logger.LogSubscribing(serviceName, $"Exchange '{config.ExchangeName}'");
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to initialize publisher", ex);
            throw;
        }
    }
    
    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) 
        where T : IDomainEvent
    {
        if (_channel == null)
            throw new InvalidOperationException("Publisher not initialized. Call InitializeAsync first.");
        
        try
        {
            var eventType = @event.GetType();

            var eventJson = JsonSerializer.Serialize(@event, eventType);
            var body = System.Text.Encoding.UTF8.GetBytes(eventJson);
            
            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                Type = @event.EventType
            };
            
            await _channel.BasicPublishAsync(
                exchange: config.ExchangeName,
                routingKey: @event.EventType,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
            
            logger.LogPublished(@event.EventType, @event.EventId, serviceName);
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to publish {typeof(T).Name}", ex);
            throw;
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.CloseAsync();
        if (_connection != null)
            await _connection.CloseAsync();
        
        _channel?.Dispose();
        _connection?.Dispose();
    }
}