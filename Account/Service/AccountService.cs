using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Common.Objects;
using Common.Packet;
using Discord;
using Grpc.Core;
using Org.BouncyCastle.Asn1.Ocsp;
using AccountSpace;

namespace Account.Service
{
    public class AccountService : AccountServerService.AccountServerServiceBase
    {
        //private EmailService _emailService;
        private Dictionary<Guid, string> _emailVerificationCodes = new Dictionary<Guid, string>();

        // 임시
        private Dictionary<string, Common.Objects.Account> _usersByEmail = new Dictionary<string, Common.Objects.Account>();
        private Dictionary<string, Common.Objects.Account> _usersByUsername = new Dictionary<string, Common.Objects.Account>();
        //
        private AccountChannel _channel = new AccountChannel();
        private Server _server;

        public AccountService(string address, int port)
        {
            _server = gRPCServerStart(address, port);
            _server.Start();
            Initialize();
            // _emailService = emailService;
        }

        public override async Task<LoginRes> Login(LoginReq request, ServerCallContext context)
        {
            var authRes = await _channel.LoginAuth(request.Username, request.Password);
            if (authRes == null)
                return null;

            return new LoginRes
            {
                Username = authRes.UserId,
                Message = authRes.Token,
                StatusCode = authRes.StatusCode,
            };
        }

        private Server gRPCServerStart(string address, int port)
        {
            Server server = new Server
            {
                Services = { AccountServerService.BindService(this) },
                Ports = { new ServerPort(address, port, ServerCredentials.Insecure) }
            };
            return server;
        }

        private void Initialize()
        {
            // TODO: 제네릭 타입 이용해서 수정할 것
            var task = ConnectServer<DBServer.DBServerClient>("DB", "127.0.0.1", 6805);
            task.Wait();
        }

        public async Task ConnectServer<T>(string serviceName, string address, int port) where T : ClientBase<T>
        {
            await _channel.AddChannel<T>(serviceName, address, port);
        }

        public bool ResetPassword(string email, string newPassword)
        {
            if (!_usersByEmail.ContainsKey(email))
            {
                return false;
            }

            var user = _usersByEmail[email];
            user.Password = HashPassword(newPassword);
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }

        private bool VerifyPassword(string hashedPassword, string password)
        {
            string passwordHash = HashPassword(password);
            return hashedPassword.Equals(passwordHash);
        }
    }
}
