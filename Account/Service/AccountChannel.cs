using Grpc.Core;
using Common;
using Common.Objects;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;
using AccountRpcService;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Account.Client;

namespace Account.Service
{
    public class AccountChannel : AccountServerService.AccountServerServiceClient
    {
        //private ConcurrentDictionary<string, List<ClientBase>> _channels = new ConcurrentDictionary<string, List<ClientBase>>();
        private ConcurrentDictionary<string, LoadBalancer<DBServiceClient>> DBServiceLB = new ConcurrentDictionary<string, LoadBalancer<DBServiceClient>>();
        private ConcurrentDictionary<string, LoadBalancer<RedisServer.RedisServerClient>> RedisServiceLB = new ConcurrentDictionary<string, LoadBalancer<RedisServer.RedisServerClient>>();

        public AccountChannel()
        {

        }

        public void AddChannel<T>(string serviceName, string address, int port) where T : ClientBase<T>
        {
            if (DBServiceLB.ContainsKey(serviceName) == false)
                DBServiceLB.TryAdd(serviceName, new LoadBalancer<DBServiceClient>(new List<DBServiceClient>()));

            DBServiceLB[serviceName].AddServer(new DBServiceClient($"{address}:{port}"));
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

            authResponse.StatusCode = (int) await GetLoginAuthInfo(request);

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

        private async Task<ResponseType> GetLoginAuthInfo(LoginReq request)
        {
            var db = DBServiceLB["DBService"].GetNextServer();
            if (db == null)
            {
                LoggingService.LogError($"not_found_db_client");
                return ResponseType.NOT_FOUND;
            }

            var response = await db.LoginAsync(request);
            if (response == null)
            {
                LoggingService.LogError($"authorization_response_is_null_error");
                return ResponseType.UNKNOWN_ERROR;
            }

            return (ResponseType)Enum.Parse(typeof(ResponseType), response.StatusCode.ToString());
        }

        private ResponseType ReConnectedAccount(ReconnectReq request)
        {
            var redis = RedisServiceLB["RedisService"].GetNextServer();
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
