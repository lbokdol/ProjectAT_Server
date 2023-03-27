using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using AccountService;
using Common.Objects;

namespace Session.Client
{
    public class AccountServiceClient
    {
        private readonly AccountService.Account.AccountClient _client;

        public AccountServiceClient(string accountServiceUrl)
        {
            var channel = GrpcChannel.ForAddress(accountServiceUrl);
            _client = new AccountService.Account.AccountClient(channel);
        }

        public async Task<LoginResponse> LoginAsync(string username, string password)
        {
            var request = new LoginRequest { Username = username, Password = password };
            return await _client.LoginAsync(request);
        }
    }
}
