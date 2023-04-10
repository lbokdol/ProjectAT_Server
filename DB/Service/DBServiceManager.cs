using Common.Interface;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using Common.Objects;
using Common;

namespace DB.Service
{
    public class AccountDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }
    }
    public class DBServiceManager : DbContext
    {
        private AccountDbContext _dbContext = new AccountDbContext(new DbContextOptions<AccountDbContext>());

        public DBServiceManager()
        {

        }

        public async Task<bool> RegisterAsync(Account account)
        {
            if (await AccountExistsAsync(account.Email, account.Username))
            {
                return false; // Email or username already exists
            }

            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            return await _dbContext.Accounts.SingleOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Account> GetAccountByUsernameAsync(string username)
        {
            return await _dbContext.Accounts.SingleOrDefaultAsync(a => a.Username == username);
        }

        public async Task<bool> UpdateAccountAsync(Account account)
        {
            _dbContext.Entry(account).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> AccountExistsAsync(string email, string username)
        {
            return await _dbContext.Accounts.AnyAsync(a => a.Email == email || a.Username == username);
        }

        public async Task<LoginResponseType> ProcessLogin(string username, string password)
        {
            // TODO: 에러코드 정리해야됨
            try
            {
                var account = await _dbContext.Accounts.SingleOrDefaultAsync(a => a.Username == username && a.PasswordHash == password);
                if (account == null)
                {
                    return LoginResponseType.NOT_FOUND;
                }

                return LoginResponseType.SUCCESS;
            }
            catch (Exception e)
            {
                LoggingService.LogError($"[Event=ProcessLogin] [Exception={e}]");
                return LoginResponseType.UNKNOWN_ERROR;
            }

        }
    }
}
