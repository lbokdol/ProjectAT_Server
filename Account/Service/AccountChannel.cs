using Grpc.Core;

namespace AccountSpace.Service
{
    public class AccountChannel : AuthService.AuthServiceClient
    {
        private Dictionary<Guid, Common.Network.Channel> _channels;

        public AccountChannel()
        {
            _channels = new Dictionary<Guid, Common.Network.Channel>();
        }

        public void AddChannel(string serviceName, string address, int port)
        {
            _channels.Add(Guid.NewGuid(), new Common.Network.Channel() { ServiceName = serviceName, Address = address, Port = port });
        }

        public string LoginAuth()
        {
            // Authentication 서비스로 보내야함 port 어떻게 받을지 고민하고
            Channel channel = new Channel("localhost", ChannelCredentials.Insecure);
            var client = new AuthService.AuthServiceClient(channel);

            var response = client.Auth(new AuthReq { Username="", Password="" });

            return response.Message;
        }
    }
}
