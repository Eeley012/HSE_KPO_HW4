namespace Payments.Service.Domain
{
    // счет, можно положить леньги и попытаться снять
    public class Account
    {
        public Guid UserId { get; private set; }
        public decimal Balance { get; private set; }

        public Account(Guid userId)
        {
            UserId = userId;
            Balance = 0;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive");
            Balance += amount;
        }

        public bool TryWithdraw(decimal amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
                return true;
            }
            return false;
        }
    }
}