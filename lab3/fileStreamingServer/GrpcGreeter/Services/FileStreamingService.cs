using Grpc.Core;
using GrpcFileStreaming;
using GrpcFileStreaming.Services;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Net;
using System;
using System.IO;
using Google.Protobuf;
using System.IO.Pipes;
using Google.Protobuf.WellKnownTypes;

namespace GrpcFileStreaming.Services
{
    public class FileStreamingService : FileStreaming.FileStreamingBase
    {
        private readonly string _resourcesDirectory = "Resources\\";
        private readonly int _packageSize = 1024;

        public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloResponse { Message = $"Hello {request.CallerName}" });
        }

        public override async Task<FileUploadResponse> SendFileToServer(IAsyncStreamReader<FileUploadRequest> requestStream, ServerCallContext context)
        {
            FileStream? fileStream = null;
            var filename = "DefaultFileName.none";
            var packageNumber = 0;

            await foreach (var response in requestStream.ReadAllAsync())
            {
                Console.WriteLine("Received package number: " + (packageNumber++));
                try
                {
                    if (response.RequestCase == FileUploadRequest.RequestOneofCase.FileData)
                    {
                        if (fileStream == null)
                        {
                            throw new Exception("Client did not send metadata to create a file!");
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


            return new FileUploadResponse
            {
                Name = filename,
                Status = Status.Success,
            };
        }

        public override async Task ReceiveFileFromServer(Empty request, IServerStreamWriter<FileUploadRequest> responseStream, ServerCallContext context)
        {
            var filename = "ServerCat.jpg";
            var pathToFile = _resourcesDirectory + filename;
            var splitted = filename.Split('.');
            var name = splitted[0];
            var type = splitted[1];
            var packageNumber = 0;

            await responseStream.WriteAsync(new FileUploadRequest
            {
                Metadata = new MetaData
                {
                    Name = name,
                    Type = type,
                },
            });
            Console.WriteLine("Sent package number: " + (packageNumber++));

            var fileStream = File.OpenRead(pathToFile);
            byte[] buffer = new byte[_packageSize];
            var bytesRead = 0;
            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await responseStream.WriteAsync(new FileUploadRequest
                {
                    FileData = new FileData
                    {
                        ByteArray = Google.Protobuf.ByteString.CopyFrom(buffer, 0, bytesRead),
                    },
                });
                Console.WriteLine("Sent package number: " + (packageNumber++));
            }

            fileStream.Close();
            Console.WriteLine("Server finished uploading a file: " + filename);
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