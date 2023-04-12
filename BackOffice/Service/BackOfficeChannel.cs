using Grpc.Core;
using BackOfficeService;
using System.Collections.Concurrent;
using Common;
using Common.Objects;

namespace BackOffice.Service
{
    public class BackOfficeChannel : DBServer.DBServerClient
    {
        private ConcurrentDictionary<string, List<ClientBase>> _channels = new ConcurrentDictionary<string, List<ClientBase>>();
        private ConcurrentDictionary<string, LoadBalancer<ClientBase>> serviceLB = new ConcurrentDictionary<string, LoadBalancer<ClientBase>>();

        public BackOfficeChannel()
        {

        }

        public async Task AddChannel<T>(string serviceName, string address, int port) where T : ClientBase<T>
        {
            var channel = new Channel($"{address}:{port}", ChannelCredentials.Insecure);
            var client = (T)Activator.CreateInstance(typeof(T), channel);

            if (_channels.ContainsKey(serviceName) == false)
            {
                _channels.TryAdd(serviceName, new List<ClientBase>());
            }

            _channels[serviceName].Add(client);

            if (serviceLB.ContainsKey(serviceName) == false)
                serviceLB.TryAdd(serviceName, new LoadBalancer<ClientBase>(_channels[serviceName]));

            serviceLB[serviceName].AddServer(client);
        }

        public async Task<RegisterRes> Register(Account account)
        {
            var registResponse = new RegisterRes()
            {
                Username = account.Username,
            };

            var request = new RegisterReq
            {
                Username = account.Username,
                Email = account.Email,
                Password = account.Password,
                Emailverified = account.EmailVerified,
            };

            registResponse.StatusCode = (int)GetRegistInfo(request);

            return registResponse;
        }

        private ResponseType GetRegistInfo(RegisterReq request)
        {

            var db = serviceLB["DB"].GetNextServer() as DBServer.DBServerClient;
            if (db == null)
            {
                LoggingService.LogError($"not_found_db_client");
                return ResponseType.NOT_FOUND;
            }

            var response = db.Register(request);
            if (response == null)
            {
                LoggingService.LogError($"register_response_is_null_error");
                return ResponseType.UNKNOWN_ERROR;
            }

            return (ResponseType)Enum.Parse(typeof(ResponseType), response.StatusCode.ToString());
        }
        /*
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
        */
    }

}
