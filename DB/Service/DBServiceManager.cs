using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace DB.Service
{
    public class DBServiceManager : IServiceManager
    {
        Greeter.GreeterClient _client;

        public DBServiceManager()
        {
            Initialize();
        }

        public void Initialize()
        {
            ConnectChannel();
        }

        public void Shutdown()
        {

        }

        public async Task SendAsync(string msg)
        {
            var reply = await _client.SayHelloAsync(new HelloRequest { Name = msg });
            Console.WriteLine(reply.Message);
        }

        public void ConnectChannel()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            _client = new Greeter.GreeterClient(channel);
        }

        public void DisconnectChannel()
        {

        }
    }
}
