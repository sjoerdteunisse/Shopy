namespace Shopy.Infrastructure.EventBus.Produce;

public class RabbitMQConfiguration
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "events";  // Generic, not "orders.events"
    public string ExchangeType { get; set; } = "fanout";
    public ushort PrefetchCount { get; set; } = 1;
    public uint ConnectionAttempts { get; set; } = 3;
}