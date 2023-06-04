using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Receive
{
    public class Message
    {
        private DateTime _timestamp;
        private string _message;
        private int _id;

        public Message(DateTime timestamp, string message, int id)
        {
            _timestamp = timestamp;
            _message = message;
            _id = id;
        }

        [Obsolete("Obsolete")]
        public static byte[] MessageToByteArray(Message msg)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, msg);
                return ms.ToArray();
            }
        }

        [Obsolete("Obsolete")]
        public static Message ByteArrayToMessage(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                if (obj is not Message message)
                {
                    Console.WriteLine("Error: obj is not Message!");
                    return null;
                }
                return message;
            }
        }

        public string ToString()
        {
            return $"Message: {_message}, id: {_id}, timestamp: {_timestamp}";
        }
    }
}
