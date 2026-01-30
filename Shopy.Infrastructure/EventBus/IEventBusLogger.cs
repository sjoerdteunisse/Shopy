namespace Shopy.Infrastructure.EventBus;

/// <summary>
/// Logging abstraction - allows any logging framework (Serilog, NLog, etc.)
/// </summary>
public interface IEventBusLogger
{
    void LogPublished(string eventType, Guid eventId, string serviceName);
    void LogSubscribing(string serviceName, string queueName);
    void LogMessageReceived(string eventType, string serviceName);
    void LogError(string message, Exception? ex = null);
    void LogProcessing(string eventType, string handlerName);
}