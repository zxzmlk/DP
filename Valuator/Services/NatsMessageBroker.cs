using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NATS.Client;

namespace Valuator
{
    public class NatsMessageBroker : IMessageBroker
    {
        // Метод для отправки сообщения в NATS
        private void SendMessage(string topic, string messageContent)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            using (IConnection natsConnection = connectionFactory.CreateConnection())
            {
                byte[] messageData = Encoding.UTF8.GetBytes(messageContent); // Кодируем сообщение в байты
                natsConnection.Publish(topic, messageData); // Отправляем сообщение
                natsConnection.Drain(); // Гарантируем, что все сообщения отправлены
            }
        }

        // Метод для асинхронной отправки сообщения
        public void Send(string topic, string messageContent)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() => SendMessage(topic, messageContent), cancellationTokenSource.Token);
        }
    }
}
