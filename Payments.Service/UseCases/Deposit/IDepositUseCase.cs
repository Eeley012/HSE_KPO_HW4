namespace Payments.Service.UseCases.Deposit;

public interface IDepositUseCase
{
    Task ExecuteAsync(DepositDto dto);
}