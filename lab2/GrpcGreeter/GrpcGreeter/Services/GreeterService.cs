using Grpc.Core;
using GrpcGreeter;
using GrpcGreeter.Services;
using System.Net;


namespace GrpcGreeter.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly double _defaultLatitude = 0.0;
        private readonly double _defaultLongtitude = 0.0;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {

            return Task.FromResult(new HelloReply
            {

                Message = "Witaj " + request.Name + "\n" + DateTime.Now.ToString("dd MMMM, HH:mm:ss")
            });
        }

        public override Task<DistanceReply> CalculateDistance1P(Distance1PRequest request, ServerCallContext context)
        {
            bool success = ValidateInput(request.Latitude, request.Longitude);
            double distance = success ? MyData.Distance(_defaultLatitude, _defaultLongtitude, request.Latitude, request.Longitude) : 0;
            
            return Task.FromResult(new DistanceReply
            {
                Success = success,
                Distance = distance});
        }

        public override Task<DistanceReply> CalculateDistance2P(Distance2PRequest request, ServerCallContext context)
        {
            bool success = ValidateInput(request.Latitude1, request.Longitude1);
            success = success || ValidateInput(request.Latitude2, request.Longitude2);
            double distance = success ? MyData.Distance(request.Latitude1, request.Longitude1, request.Latitude2, request.Longitude2) : 0;

            return Task.FromResult(new DistanceReply
            {
                Success = success,
                Distance = distance
            });

        }

        public override Task<DistanceReply> CalculateDistance3P(Distance3PRequest request, ServerCallContext context)
        {
            bool success = ValidateInput(request.Latitude1, request.Longitude1);
            success = success || ValidateInput(request.Latitude2, request.Longitude2);
            success = success || ValidateInput(request.Latitude3, request.Longitude3);
            double distance = success ? MyData.Distance(request.Latitude1, request.Longitude1, request.Latitude2, request.Longitude2) + MyData.Distance(request.Latitude2, request.Longitude2, request.Latitude3, request.Longitude3) : 0;
            return Task.FromResult(new DistanceReply
            {
                Success = success,
                Distance = distance });
        }

        private bool ValidateInput(double lat, double lon)
        {
            return (lat >= -90 && lat <= 90) && (lon >= -180 && lon <= 180);
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

        public static double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            // distance between latitudes and longitudes
            double dLat = (Math.PI / 180) * (lat2 - lat1);
            double dLon = (Math.PI / 180) * (lon2 - lon1);

            // convert to radians
            lat1 = (Math.PI / 180) * (lat1);
            lat2 = (Math.PI / 180) * (lat2);

            // apply formulae
            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Pow(Math.Sin(dLon / 2), 2) *
                       Math.Cos(lat1) * Math.Cos(lat2);
            double rad = 6371;
            double c = 2 * Math.Asin(Math.Sqrt(a));
            return rad * c;
        }
    }
}