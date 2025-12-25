namespace Payments.Service.UseCases.CreateAccount;

// интерфейс создания счета
public interface ICreateAccountUseCase
{
    Task<CreateAccountResult> ExecuteAsync(CreateAccountDto dto);
}