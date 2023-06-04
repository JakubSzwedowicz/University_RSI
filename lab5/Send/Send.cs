using System.Text;
using RabbitMQ.Client;

using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Channels;
using System.Net;

namespace ServerApp
{
    internal class Program
    {
        static void Main()
        {
            MyData.Info();
            int max = 10;
            string exchangeName = "my_exchange";
            string routingKey = "my_routing_key";
            string connectionString = "amqp://guest:guest@localhost:5672";

            Publisher publisher1 = new Publisher("Publisher1", exchangeName, routingKey, connectionString);
            Publisher publisher2 = new Publisher("Publisher2", exchangeName, routingKey, connectionString);

            var publishers = new List<Publisher>() { publisher1, publisher2 };
            var rand = new Random();
            for (int counter = 0; counter < max; counter++)
            {
                foreach (Publisher publisher in publishers)
                {
                    string message = "Hello " + counter + "!";
                    publisher.PublishMessageWithName(message);
                    Thread.Sleep(rand.Next(1000, 3000));
                }
            }
        }
    }
    internal class Publisher
    {
        private readonly string _publisherName;
        private readonly string _exchangeName;
        private readonly string _routingKey;
        private readonly string _connectionString;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Publisher(string publisherName, string exchangeName, string routingKey, string connectionString)
        {
            _publisherName = publisherName;
            _exchangeName = exchangeName;
            _routingKey = routingKey;
            _connectionString = connectionString;
            _factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct);

        }

        public void PublishMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: _exchangeName, routingKey: string.Empty, basicProperties: null, body: body);
            Console.WriteLine("publishing message: {0}", message);
        }

        public void PublishMessageWithName(string message)
        {
            var body = Encoding.UTF8.GetBytes(_publisherName + " - " + message);
            _channel.BasicPublish(exchange: _exchangeName, routingKey: string.Empty, basicProperties: null, body: body);
            Console.WriteLine(_publisherName + " - " + "publishing message: {0}", message);
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