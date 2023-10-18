namespace MessageQueue
{
    public interface IRabbitMQConsumer<T>:IMessageQueueConsumer<T>
    {
        void Consume(string queueName, Action<T> messageHandler);
    }
}