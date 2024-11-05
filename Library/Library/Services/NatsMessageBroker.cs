using System.Text;
using NATS.Client;

namespace Library
{
    public class NatsMessageBroker : IMessageBroker
    {
        public void Send(string subject, string message)
        {
            // Создание фабрики соединений NATS
            ConnectionFactory connectionFactory = new ConnectionFactory();

            // Установка соединения и отправка сообщения
            using (IConnection connection = connectionFactory.CreateConnection())
            {
                connection.Publish(subject, Encoding.UTF8.GetBytes(message));

                // Очистка и завершение соединения
                connection.Drain();
                connection.Close();
            }
        }
    }
}
