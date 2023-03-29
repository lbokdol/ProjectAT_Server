using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Objects;
using System.Security.Cryptography;

namespace Account.Manager
{
    public class AccountManager
    {
        private readonly BlockingCollection<(DBWorkType, Common.Objects.Account)> _account = new();

        public AccountManager()
        {
            Task.Run(() =>  MainThread());
        }

        public async Task CreateAccount(string username, string password, string email)
        {
            if (CheckAccountValidation(username, password, email) == false)
                return;

            var account = new Common.Objects.Account
            {
                Id = Guid.NewGuid(),
                Username = username,
                PasswordHash = HashPassword(password),
                Email = email
            };

            _account.Add((DBWorkType.Insert, account));
        }

        public async Task UpdateAccount(string username, string newPassword, string newEmail)
        {
            var account = GetAccount(username);
            if (account == null)
                return;

            //로그인 상태에서 비밀번호 변경 요청을 할 것이기 때문에 현재 비밀번호랑 대조할 필요 없을듯

            account.PasswordHash = HashPassword(newPassword);
            account.Email = newEmail;

            _account.Add((DBWorkType.Update, account));
        }

        public Common.Objects.Account GetAccount(string username)
        {
            //db에서 어카운트 가져와야됨
            return null;
        }

        private bool CheckAccountValidation(string username, string password, string email)
        {
            if (string.IsNullOrEmpty(username) == true)
                return false;

            if (string.IsNullOrEmpty(password) == true)
                return false;

            if (string.IsNullOrEmpty(email) == true)
                return false;

            // 특수 문자나 글자수 체크해야됨

            return true;
        }

        private void MainThread()
        {
            foreach(var item in _account.GetConsumingEnumerable())
            {
                //db에 upsert 요청
                switch(item.Item1)
                {
                    case DBWorkType.Insert:
                        break;
                    case DBWorkType.Update:
                        break;
                    case DBWorkType.Delete:
                        break;
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedBytes = sha256.ComputeHash(passwordBytes);
                return BitConverter.ToString(hashedBytes).Replace("-", "");
            }
        }
    }
}
