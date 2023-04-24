using Grpc.Core;
using BackOfficeRpcService;
using System.Collections.Concurrent;
using Common;
using Common.Objects;
using Account.Client;
using System.Collections.Generic;

namespace BackOffice.Service
{
    public class BackOfficeChannel : DBServerService.DBServerServiceClient
    {
        private ConcurrentDictionary<string, List<ClientBase>> _channels = new ConcurrentDictionary<string, List<ClientBase>>();
        private ConcurrentDictionary<string, LoadBalancer<DBServiceClient>> DBServiceLB = new ConcurrentDictionary<string, LoadBalancer<DBServiceClient>>();

        public BackOfficeChannel()
        {

        }

        public void AddChannel<T>(string serviceName, string address, int port) where T : ClientBase<T>
        {
            if (DBServiceLB.ContainsKey(serviceName) == false)
                DBServiceLB.TryAdd(serviceName, new LoadBalancer<DBServiceClient>(new List<DBServiceClient>()));

            DBServiceLB[serviceName].AddServer(new DBServiceClient($"{address}:{port}"));
        }
        
        public async Task<ResponseType> RegistAccount(RegisterReq request)
        {
            var db = DBServiceLB["DBService"].GetNextServer();
            if (db == null)
            {
                LoggingService.LogError($"not_found_db_client");
                return ResponseType.NOT_FOUND;
            }

            var response = await db.Register(request);
            if (response == null)
            {
                LoggingService.LogError($"register_response_is_null_error");
                return ResponseType.UNKNOWN_ERROR;
            }

            Console.WriteLine("aaaa");

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
