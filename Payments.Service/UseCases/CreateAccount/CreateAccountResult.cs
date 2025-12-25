namespace Payments.Service.UseCases.CreateAccount;

// отчет об успешном создании счета
public class CreateAccountResult
{
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
}