using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using grpcGreeterClient;
using System.Net;
using System.Runtime.Intrinsics;

namespace Program
{
    public class App
    {
        private readonly GrpcChannel _channel;
        private readonly Greeter.GreeterClient _client;
        App()
        {
            _channel = GrpcChannel.ForAddress("http://10.182.222.137:5153");
            _client = new Greeter.GreeterClient(_channel);
        }
        static void Main(string[] args)
        {
            var app = new App();
            Grpc.Core.AsyncUnaryCall<HelloReply> reply;

            MyData.Info();
            Console.WriteLine("Choose which request you want to make to the server:");
            Console.WriteLine("1. Send 1 point Malaga (36,72; -4,42)");
            Console.WriteLine("2. Send 2 points Warszawa (52,22; 21,01) - Malaga");
            Console.WriteLine("3. Send 3 points Warszawa - Malaga - Buenos Aires (-34,60; -58,38)");
            int answer = ParseIntegerInput(1, 3);
            switch (answer)
            {
                case 1:
                    {
                        var p1 = CreatePoint(1);
                        reply = app._client.SayHelloAsync(new HelloRequest { Lat1 = p1.Latitude, Lon1 = p1.Longitude });
                    }
                        break;
                case 2:
                    {
                        var p1 = CreatePoint(1);
                        var p2 = CreatePoint(2);
                        reply = app._client.SayHelloAsync(new HelloRequest { 
                        Lat1 = p1.Latitude, Lon1 = p1.Longitude, Lat2 = p2.Latitude, Lon2 = p2.Longitude});
                    }
                    break;
                case 3:
                    {
                        var p1 = CreatePoint(1);
                        var p2 = CreatePoint(2);
                        var p3 = CreatePoint(3);
                        reply = app._client.SayHelloAsync(new HelloRequest
                        {
                            Lat1 = p1.Latitude,
                            Lon1 = p1.Longitude,
                            Lat2 = p2.Latitude,
                            Lon2 = p2.Longitude,
                            Lat3 = p3.Latitude,
                            Lon3 = p3.Longitude
                        });
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid argument!");
            }
            reply.ResponseAsync.Wait();
            Console.WriteLine("The resulting distance is: " + reply.ResponseAsync.Result.Dist.ToString("0.00") + " [km]");
        }

        private static Point CreatePoint(int id)
        {
            Console.WriteLine("Insert data for " + id + " point:");
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Latitude: ");
            double lat = ParseDoubleInput(-90, 90);
            Console.Write("Longtitude: ");
            double lon = ParseDoubleInput(-180, 180);
            return new Point(name, lat, lon);
        }

        private class Point
        {
            public string Name { get; private set; }
            public double Latitude { get; private set; }
            public double Longitude { get; private set; }

            public Point(string name, double latitute, double longitude)
            {
                Name = name;
                Latitude = latitute;
                Longitude = longitude;
            }
        }

        private static double ParseDoubleInput(double lowerBound, double upperBound)
        {
            bool validInput = false;
            double parsedDouble = 0;
            while (!validInput)
            {
                Console.WriteLine("Input a value of type double from range: [" + lowerBound + ", " + upperBound + "]");
                string input = Console.ReadLine();
                if (double.TryParse(input, out parsedDouble))
                {
                    if (parsedDouble >= lowerBound && parsedDouble <= upperBound)
                    {
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("User input: " + parsedDouble + " exceeds valid range");
                    }
                }
                else
                {
                    Console.WriteLine("User input could not be parsed to double!");
                }
            }
            return parsedDouble;
        }

        private static int ParseIntegerInput(int lowerBound, int upperBound)
        {
            bool validInput = false;
            int parsedInt = 0;
            while (!validInput)
            {
                Console.WriteLine("Input a value of type integer from range: [" + lowerBound + ", " + upperBound + "]");
                string input = Console.ReadLine();
                if (int.TryParse(input, out parsedInt))
                {
                    if (parsedInt >= lowerBound && parsedInt <= upperBound)
                    {
                        validInput = true;
                    }
                    else
                    {
                        Console.WriteLine("User input: " + parsedInt + " exceeds valid range");
                    }
                }
                else
                {
                    Console.WriteLine("User input could not be parsed to integer!");
                }
            }
            return parsedInt;
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
