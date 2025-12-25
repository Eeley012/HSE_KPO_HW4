using Payments.Service.Domain;

namespace Payments.Service.Infrastructure
{
    // создание и поиск счетов в базе данных
    public class AccountRepository : IAccountRepository
    {
        private readonly DbPaymentImitationContext _dbContext;

        public AccountRepository(DbPaymentImitationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task CreateAsync(Account account)
        {
            _dbContext.ExecuteTransaction(() =>
            {
                if (!_dbContext.Accounts.Any(a => a.UserId == account.UserId))
                {
                    _dbContext.Accounts.Add(account);
                }
            });
            return Task.CompletedTask;
        }

        public Task<Account?> GetByUserIdAsync(Guid userId)
        {
            var account = _dbContext.Accounts.FirstOrDefault(a => a.UserId == userId);
            return Task.FromResult(account);
        }
        
        public Task UpdateAsync(Account account)
        {
            return Task.CompletedTask;
        }
    }
}