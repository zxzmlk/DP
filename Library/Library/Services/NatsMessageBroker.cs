using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NATS.Client;

namespace Library
{
    public class NatsMessageBroker : IMessageBroker
    {
        public void Send(string key, string message)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                c.Publish(key, Encoding.UTF8.GetBytes(message));

                c.Drain();
                c.Close();
            }
        }
    }
}