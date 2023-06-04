using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.Net.Http.Json;

namespace Receive
{
    public class Message
    {
        public DateTime _timestamp { get; set; }
        public string _message { get; set; }
        public int _id { get; set; }

        public Message()
        {
            _timestamp = DateTime.Now;
            _message = string.Empty;
            _id = 0;
        }

        public Message(DateTime timestamp, string message, int id)
        {
            _timestamp = timestamp;
            _message = message;
            _id = id;
        }

        public static byte[] MessageToByteArray(Message msg)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                IncludeFields = true
            };
            string jsonified = JsonSerializer.Serialize(msg, options);
            byte[] arr = Encoding.UTF8.GetBytes(jsonified);
            return arr;
        }

        public static Message ByteArrayToMessage(byte[] arrBytes)
        {
            string jsonified = Encoding.UTF8.GetString(arrBytes);
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                IncludeFields = true
            };
            Message? obj = JsonSerializer.Deserialize<Message>(jsonified, options);
            if (obj is not Message message)
            {
                Console.WriteLine("Error: obj is not Message!");
                return null;
            }
            return message;
        }

        public string ToString()
        {
            return $"Message: {_message}, id: {_id}, timestamp: {_timestamp}";
        }
    }
}