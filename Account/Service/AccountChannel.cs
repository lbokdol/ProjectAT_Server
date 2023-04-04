﻿using Grpc.Core;
using Common;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;
using AccountSpace;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Service
{
    public class AccountChannel : AccountServerService.AccountServerServiceClient
    {
        private ConcurrentDictionary<string, List<ClientBase>> _channels = new ConcurrentDictionary<string, List<ClientBase>>();
        private ConcurrentDictionary<string, LoadBalancer> serviceLB = new ConcurrentDictionary<string, LoadBalancer>();

        public AccountChannel()
        {

        }

        public async Task AddChannel<T>(string serviceName, string address, int port) where T : ClientBase<T>
        {
            var channel = new Channel($"{address}:{port}", ChannelCredentials.Insecure);
            var client =(T)Activator.CreateInstance(typeof(T), channel);

            if (_channels.ContainsKey(serviceName) == false)
            {
                _channels.TryAdd(serviceName, new List<ClientBase>());
            }

            _channels[serviceName].Add(client);

            if (serviceLB.ContainsKey(serviceName) == false)
                serviceLB.TryAdd(serviceName, new LoadBalancer(_channels[serviceName]));

            serviceLB[serviceName].AddServer(client);
        }

        public async Task<AuthRes> LoginAuth(string username, string password)
        {
            var client = serviceLB["DB"].GetNextServer() as DBServer.DBServerClient;
            if (client == null)
            {
                LoggingService.LogError($"not_found_db_client");

                return null;
            }
            var request = new LoginReq 
            {
                Username = username,
                Password = password,
            };
            var response = client.Login(request);
            if (response == null)
            {
                LoggingService.LogError($"authorization_response_is_null_error");
                return null;
            }

            var result = new AuthRes()
            {
                UserId = username,
                Token = Tools.TokenGenerator.GenerateToken(username, "설정 파일 또는 db에서 키 값 가져와서 적용해야됨"),
                StatusCode = response.StatusCode,
            };

            return result;
        }

    }
}
