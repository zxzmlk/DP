using NATS.Client;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text;

using System;
using System.Linq;
using System.Text;
using StackExchange.Redis;
using NATS.Client;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("RankCalculator started");

            // Создаем фабрику для соединения с NATS
            ConnectionFactory natsFactory = new ConnectionFactory();
            using IConnection natsConnection = natsFactory.CreateConnection();

            // Подключаемся к Redis
            IConnectionMultiplexer redisConnectionMultiplexer = ConnectionMultiplexer.Connect("localhost");
            IDatabase redisDatabase = redisConnectionMultiplexer.GetDatabase();

            // Подписываемся на тему "valuator.processing.rank"
            var subscription = natsConnection.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, messageArgs) =>
            {
                // Получаем идентификатор из сообщения
                string textId = Encoding.UTF8.GetString(messageArgs.Message.Data);
                Console.WriteLine($"Preparing to process text with ID: {textId}");

                // Получаем текст из Redis по идентификатору
                string textKey = "TEXT-" + textId;
                string textContent = redisDatabase.StringGet(textKey);
                double calculatedRank = 0;

                if (textContent != null)
                {
                    // Вычисляем количество символов, не являющихся буквами
                    int nonLetterCharactersCount = textContent.Count(character => !char.IsLetter(character));
                    calculatedRank = nonLetterCharactersCount / (double)textContent.Length; // Рассчитываем ранг
                }

                // Сохраняем вычисленный ранг обратно в Redis
                string rankKey = "RANK-" + textId;
                redisDatabase.StringSet(rankKey, calculatedRank.ToString("0.##"));
            });

            // Запускаем подписку
            subscription.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            // Отключаем подписку и закрываем соединения
            subscription.Unsubscribe();
            natsConnection.Drain();
            natsConnection.Close();
        }
    }
}

