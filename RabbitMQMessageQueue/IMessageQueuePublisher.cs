namespace MessageQueue
{
    public interface IMessageQueuePublisher<T>
    {
        void Publish(string exchangeName, T message);
    }
}
