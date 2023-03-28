using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Common.Objects;

namespace Session.Client
{
    public class AccountServiceClient
    {
        private readonly SessionService.SessionServiceClient _client;

        public AccountServiceClient(string accountServiceUrl)
        {
            var channel = GrpcChannel.ForAddress(accountServiceUrl);
            _client = new SessionService.SessionServiceClient(channel);
        }

        public async Task<LoginRes> LoginAsync(string username, string password)
        {
            var request = new LoginReq { Username = username, Password = password };
            return _client.Login(request);
        }
    }
}
