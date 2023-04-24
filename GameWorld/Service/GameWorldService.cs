using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWorldManager.Service
{
    public class GameWorldService
    {
        // TODO: 여기다가 다른 서비스들이랑 gRPC 통신할 수 있도록, 그리고 GameWorld의 노티를 받아서 관리할 수 있도록
        /*
        private Server gRPCServerStart(string address, int port)
        {
            Server server = new Server
            {
                Services = { DBServerService.BindService(this) },
                Ports = { new ServerPort(address, port, ServerCredentials.Insecure) }
            };

            return server;
        }
        */
    }
}
