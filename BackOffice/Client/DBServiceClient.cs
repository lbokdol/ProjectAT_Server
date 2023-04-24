using BackOfficeRpcService;
using Grpc.Core;
using Common.Objects;

namespace Account.Client
{
    public class DBServiceClient
    {
        private readonly DBServerService.DBServerServiceClient _client;

        public DBServiceClient(string dbServiceUrl)
        {
            var channel = new Channel(dbServiceUrl, ChannelCredentials.Insecure);
            _client = new DBServerService.DBServerServiceClient(channel);
        }

        public async Task<LoginRes> LoginAsync(LoginReq request)
        {
            return await _client.LoginAsync(request);
        }

        public async Task<RegisterRes> Register(RegisterReq request)
        {
            return await _client.RegisterAsync(request);
        }
    }

}