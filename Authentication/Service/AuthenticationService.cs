using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationServer;

namespace Authentication.Service
{
    public class AuthenticationService : AuthenticationServer.AuthenticationService.AuthenticationServiceBase
    {
        public AuthenticationService() 
        {

        }
        public override async Task<AuthRes> Auth(AuthReq request, ServerCallContext context)
        {
            return new AuthRes();
        }

        private void gRPCServerStart(string address, int port)
        {
            Server server = new Server
            {
                Services = { AuthenticationServer.AuthenticationService.BindService(this) },
                Ports = { new ServerPort(address, port, ServerCredentials.Insecure) }
            };

            server.Start();
        }
    }
}
