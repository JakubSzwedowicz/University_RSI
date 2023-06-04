using System.Text;
using RabbitMQ.Client;

using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Channels;
using System.Net;
using Receive;

namespace ServerApp
{
    internal class Program
    {
        static void Main()
        {
            MyData.Info();
            int max = 10;
            //string exchangeName = "hello_exchange";
            string exchangeName = string.Empty;
            string queueName = "hello_queue";
            Uri uri = new Uri("amqp://producer:producer@localhost:5672");

            Publisher publisher = new Publisher("Publisher1", exchangeName, queueName, uri);

            var rand = new Random();

            for (int i = 0; i < max; i++)
            {
                Message msg = new Message(DateTime.Now, "Publisher1 hello", i);
                publisher.PublishMessage(msg);
                Thread.Sleep(rand.Next(1000, 3000));
            }
            Console.WriteLine("Program finished...");
            Console.ReadLine();
        }
    }
    internal class Publisher
    {
        private readonly string _publisherName;
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly Uri _uri;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public Publisher(string publisherName, string exchangeName, string queueName, Uri uri)
        {
            _publisherName = publisherName;
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
        }

        public void PublishMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: _exchangeName, routingKey: _queueName, basicProperties: null, body: body);
            Console.WriteLine("publishing message: {0}", message);
        }

        public void PublishMessageWithName(string message)
        {
            var body = Encoding.UTF8.GetBytes(_publisherName + " - " + message);
            _channel.BasicPublish(exchange: _exchangeName, routingKey: _queueName, basicProperties: null, body: body);
            Console.WriteLine(_publisherName + " - " + "publishing message: {0}", message);
        }

        public void PublishMessage(Message msg)
        {
            byte[] body = Message.MessageToByteArray(msg);
            _channel.BasicPublish(exchange: _exchangeName, routingKey: _queueName, basicProperties: null, body: body);
            Console.WriteLine(_publisherName + " - " + "publishing message: {0}", msg.ToString());
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