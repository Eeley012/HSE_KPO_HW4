using Payments.Service.Domain;

namespace Payments.Service.Infrastructure
{
    // имитация базы данных для сервиса оплаты с атомарной записью
    public class DbPaymentImitationContext
    {
        public List<Account> Accounts { get; } = new();
        public List<InboxMessage> InboxMessages { get; } = new();
        public List<OutboxMessage> OutboxMessages { get; } = new();

        private readonly object _lock = new();

        public void ExecuteTransaction(Action action)
        {
            lock (_lock)
            {
                action();
            }
        }
        public List<OutboxMessage> GetUnprocessedMessages()
        {
            lock (_lock)
            {
                return OutboxMessages
                    .Where(m => m.ProcessedDate == null)
                    .ToList();
            }
        }
    }
}