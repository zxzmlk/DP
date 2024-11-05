using Library;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;
using NATS.Client;

namespace RankCalculator
{
    class Program
    {
        private static IDatabase _db;
        private static IMessageBroker _messageBroker;

        private static double CalcRank(string text) {
            double rank = 0;
            if (text != null) {
                int notLetterCharsCount = text.Where(ch => !char.IsLetter(ch)).Count();
                rank = notLetterCharsCount / (double) text.Length;
            }
            return rank;
        }

        private static void StoreRank(string id, string rank)
        {
            _db.StringSet(Const.RankKey + id, rank);
        }

        private static void PublishEventRankCalculated(string id, string rank)
        {
            EventContainer eventData = new EventContainer { Name = "RankCalculated", Id = id, Value = rank };
            _messageBroker.Send("Events", JsonSerializer.Serialize(eventData));
        }

        static void Main(string[] args)
        {
            Console.WriteLine("RankCalculator started");

            _messageBroker = new NatsMessageBroker();

            ConnectionFactory cf = new ConnectionFactory();
            using IConnection c = cf.CreateConnection();

            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect("localhost");
            _db = connectionMultiplexer.GetDatabase();

            var s = c.SubscribeAsync("valuator.processing.rank", "queue", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                Console.WriteLine("Preparing id " + id);

                string text = _db.StringGet(Const.TextKey + id);
                string rank = CalcRank(text).ToString("0.##");

                StoreRank(id, rank);
                PublishEventRankCalculated(id, rank);
            });

            s.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            s.Unsubscribe();

            c.Drain();
            c.Close();
        }
    }
}
