using Library;
using NATS.Client;
using System;
using System.Text.Json;

namespace EventsLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Events Logger Listening...");
            using (IConnection c = new ConnectionFactory().CreateConnection())
            {
                using (ISyncSubscription s = c.SubscribeSync("Events"))
                {
                    while (true)
                    {
                        Msg m = s.NextMessage();

                        EventContainer data = JsonSerializer.Deserialize<EventContainer>(m.Data);
                        Console.WriteLine($"Event: {data.Name} (Id: {data.Id}, Value: {data.Value})");
                    }
                }
            }
        }
    }
}
