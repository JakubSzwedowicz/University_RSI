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
            string exchangeName = "my_exchange";
            string queueName = "my_queue";
            string routingKey = "my_routing_key";
            string connectionString = "amqp://guest:guest@localhost:5672";

            Consumer consumer = new Consumer(exchangeName, queueName, routingKey, connectionString);
            consumer.StartConsuming();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    internal class Consumer
    {
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _routingKey;
        private readonly string _connectionString;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;

        public Consumer(string exchangeName, string queueName, string routingKey, string connectionString)
        {
            _exchangeName = exchangeName;
            _queueName = queueName;
            _routingKey = routingKey;
            _connectionString = connectionString;
            _factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName, exchange: exchangeName, routingKey: string.Empty);
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
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: _consumer);

            Console.WriteLine("Consumer started. Waiting for messages...");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
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
