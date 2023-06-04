using Google.Protobuf.WellKnownTypes;
using GrpcFileStreamingClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace fileStreamingClient
{
    public class Utils
    {

        public static double ParseDoubleInput(double lowerBound, double upperBound)
        {
            bool validInput = false;
            double parsedDouble = 0;
            while (!validInput)
            {
                Console.WriteLine("Input a value of type double from range: [" + lowerBound + ", " + upperBound + "]");
                string input = Console.ReadLine()!;
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

        public static int ParseIntegerInput(int lowerBound, int upperBound)
        {
            bool validInput = false;
            int parsedInt = 0;
            while (!validInput)
            {
                Console.WriteLine("Input a value of type integer from range: [" + lowerBound + ", " + upperBound + "]");
                string input = Console.ReadLine()!;
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
            string myIP = Dns.GetHostEntry(hostName).AddressList[3].ToString();
            Console.WriteLine("Kamil Herbetko 260389");
            Console.WriteLine("Jakub Szwedowicz 243416");
            Console.WriteLine(DateTime.Now.ToString("dd MMMM, HH:mm:ss"));
            Console.WriteLine(Environment.Version.ToString());
            Console.WriteLine(myIP);
        }
    }
}
