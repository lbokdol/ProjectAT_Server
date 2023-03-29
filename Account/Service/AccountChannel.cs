using Grpc.Core;
using System.Runtime.CompilerServices;

namespace AccountSpace.Service
{
    public class AccountChannel : AuthService.AuthServiceClient
    {
        private Dictionary<Guid, Common.Network.Channel> _channels;
        private string _authService = "";
        public AccountChannel()
        {
            _channels = new Dictionary<Guid, Common.Network.Channel>();
        }

        public void AddChannel(string serviceName, string address, int port)
        {
            _channels.Add(Guid.NewGuid(), new Common.Network.Channel() { ServiceName = serviceName, Address = address, Port = port });
        }

        public string LoginAuth(string username, string password)
        {
            Channel channel = new Channel(_authService, ChannelCredentials.Insecure);
            var client = new AuthService.AuthServiceClient(channel);

            var response = client.Auth(new AuthReq { Username= username, Password= password });

            return response.Message;
        }
    }
}
