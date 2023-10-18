using System.Security.Cryptography;

namespace MessageQueue
{
    public interface IMessageQueueConsumer<T>
    {
        void Consume(string queueName, Action<T> messageHandler);
    }
}
