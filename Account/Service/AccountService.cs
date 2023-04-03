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

        private Dictionary<string, Common.Objects.Account> _usersByEmail = new Dictionary<string, Common.Objects.Account>();
        private Dictionary<string, Common.Objects.Account> _usersByUsername = new Dictionary<string, Common.Objects.Account>();

        private AccountChannel _channel = new AccountChannel();
        private Server _server;

        public AccountService(string address, int port)
        {
            _server = gRPCServerStart(address, port);
            _server.Start();
            Initialize();
            // _emailService = emailService;
        }

        public bool Register(string username, string email, string password)
        {
            if (!ValidateUsername(username) || !ValidateEmail(email) || !ValidatePassword(password))
            {
                return false;
            }

            SendEmailVerification(email);

            var user = new Common.Objects.Account
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password)
            };

            _usersByEmail[email] = user;
            _usersByUsername[username] = user;
            return true;
        }

        public override async Task<LoginRes> Login(LoginReq request, ServerCallContext context)
        {
            var authRes = await _channel.LoginAuth(request.Username, request.Password);

            return new LoginRes
            {
                Username = authRes.UserId,
                Message = authRes.Token,
                StatusCode = authRes.StatusCode,
            };
        }
        
        public override Task<ConnectRes> Connect(ConnectReq request, ServerCallContext context)
        {
            return Task.FromResult(new ConnectRes
            {
                Message = "Connect!"
            });
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
            ConnectServer<>("DB", "127.0.0.1", 6805);
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
            user.PasswordHash = HashPassword(newPassword);
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

        public bool ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            return Regex.IsMatch(username, @"^[a-zA-Z0-9]{3,30}$");
        }

        public bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            // 최소 8자, 최소 하나의 대문자, 하나의 소문자, 하나의 숫자, 하나의 특수문자를 포함
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
        }

        private void SendEmailVerification(string email)
        {
            var user = _usersByEmail[email];
            var verificationCode = GenerateVerificationCode();
            _emailVerificationCodes[user.Id] = verificationCode;

            string subject = "RPG Game - Email Verification";
            string body = $"Hello {user.Username},\n\nPlease use the following verification code to complete your registration:\n\n{verificationCode}\n\nThank you,\nThe RPG Game Team";

            //_emailService.SendEmail(email, user.Username, subject, body);
        }

        public bool VerifyEmail(Guid userId, string verificationCode)
        {
            if (!_emailVerificationCodes.ContainsKey(userId))
            {
                return false;
            }

            bool isVerified = _emailVerificationCodes[userId] == verificationCode;
            if (isVerified)
            {
                _emailVerificationCodes.Remove(userId);
            }
            return isVerified;
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
