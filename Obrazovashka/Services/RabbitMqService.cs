// RabbitMqService.cs - версия из загруженных файлов

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Obrazovashka.Services
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqService(string hostName)
        {
            var factory = new ConnectionFactory { HostName = hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "statistics",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void PublishMessage(string queueName, object message)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(message));
                _channel.BasicPublish(exchange: "",
                                      routingKey: queueName,
                                      basicProperties: null,
                                      body: body);
                Console.WriteLine($"[RabbitMQ] Сообщение отправлено в очередь '{queueName}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RabbitMQ] Ошибка отправки сообщения: {ex.Message}");
                throw;
            }
        }

        public void ListenForMessages(string queueName, Action<string> onMessageReceived)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    onMessageReceived(message);
                    Console.WriteLine($"[RabbitMQ] Сообщение получено из очереди '{queueName}': {message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RabbitMQ] Ошибка обработки сообщения: {ex.Message}");
                }
            };

            _channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
