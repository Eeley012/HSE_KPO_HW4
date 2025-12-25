using Payments.Service.Domain;

namespace Payments.Service.UseCases.CreateAccount
{
    // создание счета и отправка в имитацию БД
    public class CreateAccountUseCase : ICreateAccountUseCase
    {
        private readonly IAccountRepository _repository;

        public CreateAccountUseCase(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateAccountResult> ExecuteAsync(CreateAccountDto dto)
        {
            var existingAccount = await _repository.GetByUserIdAsync(dto.UserId);
            if (existingAccount != null)
            {
                throw new InvalidOperationException($"Account for user {dto.UserId} already exists.");
            }
            var account = new Account(dto.UserId);
            
            if (dto.InitialBalance > 0)
            {
                account.Deposit(dto.InitialBalance);
            }
            await _repository.CreateAsync(account);

            return new CreateAccountResult
            {
                UserId = account.UserId,
                Balance = account.Balance
            };
        }
    }
}