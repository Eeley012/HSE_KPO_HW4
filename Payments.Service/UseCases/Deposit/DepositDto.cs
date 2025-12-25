namespace Payments.Service.UseCases.Deposit;

public class DepositDto
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
}