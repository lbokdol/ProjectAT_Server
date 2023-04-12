using Grpc.Core;
using Common;
using Common.Objects;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;
using AccountSpace;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Service
{
    public class AccountChannel : AccountServerService.AccountServerServiceClient
    {
        private ConcurrentDictionary<string, List<ClientBase>> _channels = new ConcurrentDictionary<string, List<ClientBase>>();
        private ConcurrentDictionary<string, LoadBalancer<ClientBase>> serviceLB = new ConcurrentDictionary<string, LoadBalancer<ClientBase>>();

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
                serviceLB.TryAdd(serviceName, new LoadBalancer<ClientBase>(_channels[serviceName]));

            serviceLB[serviceName].AddServer(client);
        }

        public async Task<AuthRes> LoginAuth(string username, string password)
        {
            var authResponse = new AuthRes()
            {
                UserId = username,
                Token = Tools.TokenGenerator.GenerateToken($"{username}:{password}", "설정 파일 또는 db에서 키 값 가져와서 적용해야됨"),
            };

            var request = new LoginReq
            {
                Username = username,
                Password = password,
            };

            authResponse.StatusCode = (int)GetLoginAuthInfo(request);

            return authResponse;
        }

        // reconnectKey은 로그인해서 받은 토큰으로 Wolrd에 접속하게 되면 생성하도록

        public async Task<AuthRes> ReconnectAuth(string username, string reconnectKey)
        {
            // 재연결 객체는 새로 만들어야될 듯..?
            var authResponse = new AuthRes()
            {
                UserId = username,
                Token = reconnectKey,
                StatusCode = (int)ResponseType.SUCCESS,
            };

            var request = new ReconnectReq
            {
                Username = username,
                Reconnectkey = reconnectKey,
            };

            authResponse.StatusCode = (int)ReConnectedAccount(request);

            return authResponse;
        }

        private ResponseType GetLoginAuthInfo(LoginReq request)
        {

            var db = serviceLB["DB"].GetNextServer() as DBServer.DBServerClient;
            if (db == null)
            {
                LoggingService.LogError($"not_found_db_client");
                return ResponseType.NOT_FOUND;
            }

            var response = db.Login(request);
            if (response == null)
            {
                LoggingService.LogError($"authorization_response_is_null_error");
                return ResponseType.UNKNOWN_ERROR;
            }

            return (ResponseType)Enum.Parse(typeof(ResponseType), response.StatusCode.ToString());
        }

        private ResponseType ReConnectedAccount(ReconnectReq request)
        {
            var redis = serviceLB["Redis"].GetNextServer() as RedisServer.RedisServerClient;
            if (redis == null)
            {
                LoggingService.LogError($"not_found_redis_client");

                return ResponseType.NOT_FOUND;
            }

            var response = redis.Reconnect(request);
            if (response == null)
            {
                LoggingService.LogError($"redis_response_is_null_error");
                return ResponseType.UNKNOWN_ERROR;
            }

            return (ResponseType)Enum.Parse(typeof(ResponseType), response.StatusCode.ToString());
        }


    }
}
