using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chain
{
    class Program
    {
        private static Socket sender;
        private static Socket listener;
        private static int x;
        static void Main(string[] args)
        {
            // порт, на котором текущий процесс будет слушать
            int listenPort = Convert.ToInt32(args[0]);

            // адрес следующего процесса (например, "localhost")
            string writerAddress = args[1];

            // порт следующего процесса
            var writerPort = Convert.ToInt32(args[2]);

            // Проверяем, является ли процесс инициатором
            bool isInitiator = args.Length == 4 && args[3] == "true";

            // Настраиваем сокеты (слушатель и отправитель)
            InitSockets(listenPort, writerAddress, writerPort);

            x = Convert.ToInt32(Console.ReadLine());

            if (isInitiator)
            {
                WorkAsInitiator();
            }
            else
            {
                WorkAsNormalProcess();
            }

            // Завершаем работу сокета
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();

            Console.ReadKey();
        }

        private static int ReadIntFromSocket(Socket handler)
        {
            byte[] buf = new byte[1024];
            string data = null;
            do
            {
                int bytes = handler.Receive(buf);
                data += Encoding.UTF8.GetString(buf, 0, bytes);
            }
            while (handler.Available > 0);
            return Convert.ToInt32(data);
        }

        private static void SendIntToSocket(Socket handler, int number)
        {
            sender.Send(Encoding.UTF8.GetBytes("" + number));
        }

        private static void WorkAsInitiator()
        {
            // Шаг 1: Отправляем свое значение x следующему процессу
            SendIntToSocket(sender, x);

            // Шаг 2: Ждем, пока значение пройдет через все кольцо 
            // и вернется к нам от последнего процесса
            Socket handler = listener.Accept();
            int y = ReadIntFromSocket(handler);
            Console.WriteLine(y);

            // Шаг 3: Отправляем максимальное значение дальше по кольцу
            SendIntToSocket(sender, Math.Max(x, y));

            // Закрываем соединение
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private static void WorkAsNormalProcess()
        {
            // Шаг 1: Получаем значение от предыдущего процесса
            Socket handler = listener.Accept();
            int y = ReadIntFromSocket(handler);

            // Шаг 2: Сравниваем со своим значением и отправляем максимум дальше
            SendIntToSocket(sender, Math.Max(x, y));

            // Шаг 3: Получаем финальное максимальное значение
            y = ReadIntFromSocket(handler);
            Console.WriteLine(y);

            // Шаг 4: Передаем это значение дальше по кольцу
            SendIntToSocket(sender, y);

            // Закрываем соединение
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private static void InitSockets(int listenPort, string writerAddress, int writerPort)
        {
            // 1. IPAddress.Any (0.0.0.0) - слушать на всех сетевых интерфейсах компьютера
            IPAddress listenIpAddress = IPAddress.Any;

            // 2. Создаем точку входа для прослушивания
            IPEndPoint localEP = new IPEndPoint(listenIpAddress, listenPort);

            // 3. Создаем сокет для прослушивания
            listener = new Socket(
                listenIpAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            // 4. Привязываем сокет к точке входа - теперь он слушает на 0.0.0.0:listenPort
            listener.Bind(localEP);

            // 5. Начинаем принимать входящие соединения
            listener.Listen(10);

            // 1. Упрощаем получение IP-адреса для подключения
            IPAddress ipAddress = IPAddress.Parse(writerAddress);

            // 2. Создаем точку подключения
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, writerPort);

            // 3. Создаем сокет для отправки
            sender = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            // 4. Подключаемся к удаленной точке
            ConnectWriter(remoteEP);
        }

        private static void ConnectWriter(IPEndPoint remoteEP)
        {
            while (true)
            {
                try
                {
                    sender.Connect(remoteEP);
                    return;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Ошибка подключения: {ex.Message}");
                    Thread.Sleep(1000);
                }
            }
        }

    }
}
