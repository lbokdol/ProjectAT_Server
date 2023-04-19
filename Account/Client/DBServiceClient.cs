using AccountRpcService;
using Common.Network;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

}