namespace MessageQueue
{
        public interface IRabbitMQPublisher<T>:IMessageQueuePublisher<T>
        {
            void Publish(string exchangeName, T message);
        }
}