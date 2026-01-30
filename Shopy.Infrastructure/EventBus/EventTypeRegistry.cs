using System.Reflection;
using Shopy.Core.Events;

namespace Shopy.Infrastructure.EventBus;

public class EventTypeRegistry
{
    private readonly Dictionary<string, Type> _eventTypes = new();
    
    public EventTypeRegistry()
    {
        // Register all event types from Shared assembly
        RegisterEventsFromAssembly(typeof(IDomainEvent).Assembly);
    }
    
    private void RegisterEventsFromAssembly(Assembly assembly)
    {
        var eventType = typeof(IDomainEvent);
        
        var types = assembly.GetTypes()
            .Where(t => eventType.IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .ToList();
        
        foreach (var type in types)
        {
            _eventTypes[type.Name] = type;
        }
    }
    
    /// <summary>
    /// Get concrete type for event name
    /// </summary>
    public Type? GetEventType(string eventTypeName)
    {
        return _eventTypes.GetValueOrDefault(eventTypeName);
    }
    
    /// <summary>
    /// Register custom event type (useful for tests)
    /// </summary>
    public void Register<T>(string eventTypeName) where T : IDomainEvent
    {
        _eventTypes[eventTypeName] = typeof(T);
    }
}