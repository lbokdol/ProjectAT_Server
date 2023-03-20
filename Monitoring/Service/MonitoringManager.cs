using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grpc.Net.Client;

namespace Monitoring.Service
{
    public class MonitoringManager : IServiceManager
    {
        Greeter.GreeterClient _client;

        public MonitoringManager()
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
