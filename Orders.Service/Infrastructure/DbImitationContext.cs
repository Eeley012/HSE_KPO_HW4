using Orders.Service.Domain;

namespace Orders.Service.Infrastructure
{
    // имитация базы данных, обеспечиваем атомарность записи данных lock-ом
    public class DbImitationContext
    {
        public List<Order> Orders { get; } = new();
        public List<OutboxMessage> OutboxMessages { get; } = new();
        
        private readonly object _lock = new();
        
        public void ExecuteTransaction(Action action)
        {
            // атомарно делаем что-то
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