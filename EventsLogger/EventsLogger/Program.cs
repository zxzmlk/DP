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
            Console.WriteLine("Events Logger is now listening for messages...");

            using (IConnection connection = new ConnectionFactory().CreateConnection())
            {
                using (ISyncSubscription subscription = connection.SubscribeSync("Events"))
                {
                    while (true)
                    {
                        // Получение следующего сообщения
                        Msg message = subscription.NextMessage();

                        // Десериализация данных события
                        EventContainer eventData = JsonSerializer.Deserialize<EventContainer>(message.Data);
                        Console.WriteLine($"Received Event: {eventData.Name} (ID: {eventData.Id}, Value: {eventData.Value})");
                    }
                }
            }
        }
    }
}
