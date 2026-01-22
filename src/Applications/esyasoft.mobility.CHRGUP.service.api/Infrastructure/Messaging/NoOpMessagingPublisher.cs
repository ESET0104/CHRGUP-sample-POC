using esyasoft.mobility.CHRGUP.service.api.Interfaces;

namespace esyasoft.mobility.CHRGUP.service.api.Infrastructure.Messaging
{
    /// <summary>
    /// Development-only messaging publisher.
    /// Does not publish to RabbitMQ.
    /// This exists so API can run independently.
    /// Will be replaced by RabbitMqMessagingPublisher.
    /// </summary>
    public class NoOpMessagingPublisher : IMessagingPublisher
    {
        public Task PublishAsync<T>(T message)
        {
            Console.WriteLine(
            $"[PUBLISHER] {typeof(T).Name}: {System.Text.Json.JsonSerializer.Serialize(message)}"
        );
            return Task.CompletedTask;
        }
    }

}
