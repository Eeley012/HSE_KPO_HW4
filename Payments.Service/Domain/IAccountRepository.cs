namespace Payments.Service.Domain
{
    // интерфейс хранилища счетов
    public interface IAccountRepository
    {
        Task CreateAsync(Account account);
        Task<Account?> GetByUserIdAsync(Guid userId);
        Task UpdateAsync(Account account);
    }
}