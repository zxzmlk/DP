using Library;
using NATS.Client;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace RankCalculator
{
    class Program
    {
        private static IMessageBroker _messageBroker;
        private static RedisStorage _storage;

        private static double CalcRank(string text) {
            double rank = 0;
            if (text != null) {
                int notLetterCharsCount = text.Where(ch => !char.IsLetter(ch)).Count();
                rank = notLetterCharsCount / (double) text.Length;
            }
            return rank;
        }

        private static void StoreRank(string segmentId, string id, string rank)
        {
            _storage.Store(segmentId, Const.RankKey + id, rank);
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
            _storage = new RedisStorage();

            ConnectionFactory cf = new ConnectionFactory();
            using IConnection c = cf.CreateConnection();

            var s = c.SubscribeAsync("valuator.processing.rank", "queue", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                Console.WriteLine("Preparing id " + id);

                string segmentId = _storage.GetSegmentId(id);

                // Получаем текст из соответствующего сегмента по id
                string text = _storage.Load(segmentId, Const.TextKey + id);
                string rank = CalcRank(text).ToString("0.##");

                // Сохраняем Rank в соответствующий сегмент
                StoreRank(segmentId, id, rank);
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
