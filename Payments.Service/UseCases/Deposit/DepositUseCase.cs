using System;
using System.Threading.Tasks;
using Payments.Service.Domain;

namespace Payments.Service.UseCases.Deposit
{

    public class DepositUseCase : IDepositUseCase
    {
        private readonly IAccountRepository _repository;

        public DepositUseCase(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(DepositDto dto)
        {
            var account = await _repository.GetByUserIdAsync(dto.UserId);
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            account.Deposit(dto.Amount);
            await _repository.UpdateAsync(account);
        }
    }
}