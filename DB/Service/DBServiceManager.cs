using Common.Interface;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
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

    public class DBServiceManager
    {
        private AccountDbContext _dbContext;// = new AccountDbContext(new DbContextOptions<AccountDbContext>());

        public DBServiceManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            // TODO: db 정보는 설정파일로 옮길 것
            var dbInfo = "Server=localhost;Port=3307;Database=project_at;Uid=root;Pwd=1234;";
            var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();
            optionsBuilder.UseMySQL(dbInfo);

            _dbContext = new AccountDbContext(optionsBuilder.Options);
            //_dbContext.Database.EnsureCreated();
        }

        public async Task<ResponseType> RegisterAsync(Account account)
        {
            if (await AccountExistsAsync(account.Email, account.Username))
            {
                return ResponseType.ALREADYEXIST;
            }

            _dbContext.Accounts.Add(account);

            var response = await _dbContext.SaveChangesAsync();

            return ResponseType.SUCCESS;
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

        public async Task<ResponseType> ProcessLogin(string username, string password)
        {
            // TODO: 에러코드 정리해야됨
            try
            {
                var account = await _dbContext.Accounts.SingleOrDefaultAsync(a => a.Username == username && a.Password == password);
                if (account == null)
                {
                    return ResponseType.NOT_FOUND;
                }
                Console.WriteLine("3===");
                return ResponseType.SUCCESS;
            }
            catch (Exception e)
            {
                LoggingService.LogError($"[Event=ProcessLogin] [Exception={e}]");
                return ResponseType.UNKNOWN_ERROR;
            }
        }
    }
}
