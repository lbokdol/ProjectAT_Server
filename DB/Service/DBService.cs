using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using Common.Objects;
using Grpc.Core;
using System.Net;
using DBRpcService;

namespace DB.Service
{
    public class DBService : DBServerService.DBServerServiceBase
    {
        private DBServiceManager _dbServiceManager = new DBServiceManager();
        private Server _server;
        public DBService(string address, int port) 
        {
            _server = gRPCServerStart(address, port);
            _server.Start();
        }

        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            return await _dbServiceManager.GetAccountByEmailAsync(email);
        }

        public async Task<Account> GetAccountByUsernameAsync(string username)
        {
            return await _dbServiceManager.GetAccountByUsernameAsync(username);
        }

        public async Task<bool> UpdateAccountAsync(Account account)
        {
            return await _dbServiceManager.UpdateAccountAsync(account);
        }

        public override async Task<RegisterRes> Register(RegisterReq account, ServerCallContext context)
        {
            var newAccount = new Account
            {
                Id = Guid.Parse(account.Id),
                Username = account.Username,
                Email = account.Email,
                Password = account.Password,
                EmailVerified = account.Emailverified,
            };
            var resultCode = await _dbServiceManager.RegisterAsync(newAccount);

            RegisterRes response = new RegisterRes()
            {
                Username = account.Username,
                StatusCode = (int)resultCode,
            };

            return response;
        }

        public override async Task<LoginRes> Login(LoginReq request, ServerCallContext context)
        {
            var result = await _dbServiceManager.ProcessLogin(request.Username, request.Password);

            return new LoginRes
            {
                Username = request.Username,
                Message = "",
                StatusCode = (int)result,
            };
        }

        private Server gRPCServerStart(string address, int port)
        {
            Server server = new Server
            {
                Services = { DBServerService.BindService(this) },
                Ports = { new ServerPort(address, port, ServerCredentials.Insecure) }
            };

            return server;
        }
    }
}
