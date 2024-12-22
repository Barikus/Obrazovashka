using Obrazovashka.DTOs;
using Obrazovashka.Results;

namespace Obrazovashka.Services
{
    public interface IRabbitMqService : IDisposable
    {
        void PublishMessage(string queueName, object message);
        void ListenForMessages(string queueName, Action<string> onMessageReceived);
    }
}
