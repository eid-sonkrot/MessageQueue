using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;

namespace MessageQueue
{
    public class RabbitMQConsumer<T> : IRabbitMQConsumer<T>
    {
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQConsumer(string hostName, string userName, string password)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            createChannel(factory);
        }
        private void createChannel(ConnectionFactory factory)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to create a RabbitMQ connection: {ex.Message}");
                throw;
            }
        }
        public void Consume(string queueName, Action<T> messageHandler)
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();

                try
                {
                    var message = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(body));
                    messageHandler(message);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to process received message: {ex.Message}");
                }
            };
            _channel.BasicConsume(queueName, true, consumer);
        }
        public void Dispose()
        {
            try
            {
                _channel.Dispose();
                _connection.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to dispose of RabbitMQ connection: {ex.Message}");
                throw;
            }
        }
    }
}