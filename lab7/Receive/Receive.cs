using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Channels;
using System.Net;

namespace ClientApp
{
    internal class Program
    {
        static void Main()
        {
            MyData.Info();
            //string exchangeName = "hello_exchange";
            string exchangeName = string.Empty;
            string queueName = "hello_queue";
            Uri uri = new Uri("amqp://consumer:consumer@localhost:5672");

            Consumer consumer = new Consumer(exchangeName, queueName, uri);
            consumer.StartConsuming();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    internal class Consumer
    {
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly Uri _uri;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;

        public Consumer(string exchangeName, string queueName, Uri uri)
        {
            _exchangeName = exchangeName;
            _queueName = queueName;
            _uri = uri;
            _factory = new ConnectionFactory
            {
                Uri = _uri,
                VirtualHost = "/"
            };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (model, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Message received: {0}", message);
            };

        }

        public void StartConsuming()
        {
            bool running = true;
            _channel.BasicConsume(queue: _queueName,
                autoAck: true,
                consumer: _consumer);

            Console.WriteLine("Consumer started. Waiting for messages...");
            Console.WriteLine("Enter 'exit' to close the program...");
            while (running)
            {
                if (Console.ReadLine() == "exit")
                {
                    running = false;
                }
            }
        }
    }

    public class MyData
    {
        public static void Info()
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[3].ToString();
            Console.WriteLine("Kamil Herbetko 260389");
            Console.WriteLine("Jakub Szwedowicz 243416");
            Console.WriteLine(DateTime.Now.ToString("dd MMMM, HH:mm:ss"));
            Console.WriteLine(Environment.Version.ToString());
            Console.WriteLine(myIP);
        }
    }
}
