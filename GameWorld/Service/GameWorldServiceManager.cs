using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameWorldManagerRpcService;

namespace GameWorldManager.Service
{
    public class GameWorldServiceManager : IServiceManager
    {
        Greeter.GreeterClient _client;

        public GameWorldServiceManager()
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

        // TODO: 이전에 작성한 Service들과 같이 gRPC 서버를 구동시키는 코드를 작성해야 한다.
        public void ConnectChannel()
        {
            //var channel = GrpcChannel.ForAddress("https://localhost:5001");
            //_client = new Greeter.GreeterClient(channel);
        }

        public void DisconnectChannel()
        {

        }
    }
}
