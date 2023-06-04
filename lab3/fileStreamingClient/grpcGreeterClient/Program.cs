using fileStreamingClient;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcFileStreamingClient;
using System.Net;
using System.Xml.Linq;

namespace Program
{
    public class App
    {
        private readonly GrpcChannel _channel;
        private readonly FileStreaming.FileStreamingClient _client;
        private readonly string _resourcesDirectory;
        private readonly int _packageSize = 2 * 1024;

        App()
        {
            _channel = GrpcChannel.ForAddress("http://10.182.162.78:5153");
            //_channel = GrpcChannel.ForAddress("http:localhost:5153");
            _client = new FileStreaming.FileStreamingClient(_channel);
            string workingDirectory = Environment.CurrentDirectory;
            _resourcesDirectory = Directory.GetParent(workingDirectory)!.Parent!.Parent!.FullName + "\\Resources\\";
        }
        public static async Task Main(string[] args)
        {
            try
            {
                MyData.Info();
                var app = new App();
                await app.RunClient();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task RunClient()
        {
            var running = true;

            while (running)
            {

                Console.WriteLine("Choose which request you want to make to the server:");
                Console.WriteLine("0. Close program \n1. Test connection \n2. Send cat image \n3. Receive file from server");
                int answer = Utils.ParseIntegerInput(0, 3);
                switch (answer)
                {
                    case 0:
                        running = false;
                        break;
                    case 1:
                        await TestConnection();
                        break;
                    case 2:
                        await SendFile();
                        break;
                    case 3:
                        await ReceiveFile();
                        break;
                    default:
                        throw new ArgumentException("Invalid argument!");
                }
            }
        }

        private async Task TestConnection()
        {
            var message = await _client.SayHelloAsync(new HelloRequest { CallerName = "Jakub", });
            Console.WriteLine("Received response: " + message);
        }

        private async Task SendFile(string filename = "ClientWallpaper.jpg")
        {
            var pathToFile = _resourcesDirectory + filename;
            var splitted = filename.Split('.');
            var name = splitted[0];
            var type = splitted[1];

            var fileStream = File.OpenRead(pathToFile);
            byte[] buffer = new byte[_packageSize];
            int bytesRead = 0;
            var fileStreamingService = _client.SendFileToServer();
            var packageNumber = 0;

            await fileStreamingService.RequestStream.WriteAsync(new FileUploadRequest
            {
                Metadata = new MetaData
                {
                    Name = name,
                    Type = type,
                },
            });
            Console.WriteLine("Sent package number: " + (packageNumber++));
            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStreamingService.RequestStream.WriteAsync(new FileUploadRequest
                {
                    FileData = new FileData
                    {
                        ByteArray = Google.Protobuf.ByteString.CopyFrom(buffer, 0, bytesRead),
                    },
                });
                Console.WriteLine("Sent package number: " + (packageNumber++));
            }
            fileStream.Close();

            await fileStreamingService.RequestStream.CompleteAsync();
            var resposne = await fileStreamingService.ResponseAsync;
            Console.WriteLine("Resposne from server: " + resposne.ToString());
        }

        private async Task ReceiveFile()
        {
            FileStream? fileStream = null;
            var filename = "DefaultFileName.none";

            var receiveFileService = _client.ReceiveFileFromServer(new Empty { });
            var packageNumber = 0;

            await foreach (var response in receiveFileService.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine("Received package number: " + (packageNumber++));
                try
                {
                    if (response.RequestCase == FileUploadRequest.RequestOneofCase.FileData)
                    {
                        if (fileStream == null)
                        {
                            throw new Exception("Server didn't send metadata to create a file!");
                        }

                        var fileData = response.FileData;
                        await fileStream.WriteAsync(fileData.ByteArray.ToByteArray());
                    }
                    else if (response.RequestCase == FileUploadRequest.RequestOneofCase.Metadata)
                    {
                        var metaData = response.Metadata;
                        filename = metaData.Name + "." + metaData.Type;
                        fileStream = File.Create(_resourcesDirectory + filename);
                    }
                    else
                    {
                        throw new ArgumentException("Unknown FileUpload request");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            fileStream?.Close();

            Console.WriteLine("File of name " + filename + " was received from the server");
        }

    }
}
