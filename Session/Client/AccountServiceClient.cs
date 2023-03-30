using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Objects;
using Grpc.Core;
using AccountSpace;

namespace Session.Client
{
    public class AccountServiceClient
    {
        private readonly AccountServerService.AccountServerServiceClient _client;

        public AccountServiceClient(string accountServiceUrl)
        {
            var channel = new Channel(accountServiceUrl, ChannelCredentials.Insecure);
            _client = new AccountServerService.AccountServerServiceClient(channel);
        }

        public async Task<LoginRes> LoginAsync(LoginReq request)
        {
            return await _client.LoginAsync(request);
        }
    }
}
