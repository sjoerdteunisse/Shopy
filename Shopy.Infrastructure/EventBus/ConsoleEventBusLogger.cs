namespace Shopy.Infrastructure.EventBus;

/// <summary>
/// Simple console implementation
/// </summary>
public class ConsoleEventBusLogger(string serviceName) : IEventBusLogger
{
    public void LogPublished(string eventType, Guid eventId, string serviceName)
    {
        Console.WriteLine($"[{serviceName}] Published {eventType} (EventId: {eventId:D})");
        Console.ResetColor();
    }
    
    public void LogSubscribing(string serviceName, string queueName)
    {
        Console.WriteLine($"[{serviceName}] Subscribed to queue '{queueName}'");
        Console.ResetColor();
    }
    
    public void LogMessageReceived(string eventType, string serviceName)
    {
        Console.WriteLine($"[{serviceName}] Received: {eventType}");
        Console.ResetColor();
    }
    
    public void LogError(string message, Exception? ex = null)
    {
        Console.WriteLine($"[{serviceName}] ERROR: {message}");
        
        if (ex != null)
            Console.WriteLine($"  Exception: {ex.Message}");
        
        Console.ResetColor();
    }
    
    public void LogProcessing(string eventType, string handlerName)
    {
        Console.WriteLine($"Processing {eventType} with {handlerName}");
    }
}
