using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;
using System.Text;

namespace MessageQueue
{
    public class RabbitMQPublisher<T> : IRabbitMQPublisher<T>
    {
        private  IModel _channel;
        private  IConnection _connection;

        public RabbitMQPublisher(string hostName, string userName, string password)
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
        public void Publish(string exchangeName, T message)
        {
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
            try
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                _channel.BasicPublish(exchangeName, "", null, body);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to publish message: {ex.Message}");
                throw;
            }
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