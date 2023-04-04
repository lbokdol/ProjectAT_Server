using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using Common.Objects;
using DB;
using Grpc.Core;
using System.Net;

namespace DB.Service
{
    public class DBService : DBServer.DBServerBase
    {
        private DBServiceManager _dbServiceManager = new DBServiceManager();
        private Server _server;
        public DBService(string address, int port) 
        {
            _server = gRPCServerStart(address, port);
            _server.Start();
        }

        public async Task<bool> RegisterAsync(Common.Objects.Account account)
        {
            return await _dbServiceManager.RegisterAsync(account);
        }

        public async Task<Common.Objects.Account> GetAccountByEmailAsync(string email)
        {
            return await _dbServiceManager.GetAccountByEmailAsync(email);
        }

        public async Task<Common.Objects.Account> GetAccountByUsernameAsync(string username)
        {
            return await _dbServiceManager.GetAccountByUsernameAsync(username);
        }

        public async Task<bool> UpdateAccountAsync(Common.Objects.Account account)
        {
            return await _dbServiceManager.UpdateAccountAsync(account);
        }

        public override async Task<LoginRes> Login(LoginReq request, ServerCallContext context)
        {
            var result = await _dbServiceManager.ProcessLogin(request.Username, request.Password);

            //에러코드 정리되면 메시지랑 Statuscode 넣어줘야함
            return new LoginRes
            {
                Username = request.Username,
                Message = "",
                StatusCode = result,
            };
        }

        private Server gRPCServerStart(string address, int port)
        {
            Server server = new Server
            {
                Services = { DBServer.BindService(this) },
                Ports = { new ServerPort(address, port, ServerCredentials.Insecure) }
            };

            return server;
        }
    }
}
