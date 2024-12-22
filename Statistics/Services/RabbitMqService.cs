using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Statistics.Services
{
    public class RabbitMqService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqService(string hostName)
        {
            var factory = new ConnectionFactory() { HostName = hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "statistics", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void ListenForMessages(Func<string, Task> onMessageReceived)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    await onMessageReceived(message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                    Console.WriteLine($"Сообщение обработано: {message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка обработки сообщения: {ex.Message}");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: "statistics", autoAck: false, consumer: consumer);
        }
    }
}
