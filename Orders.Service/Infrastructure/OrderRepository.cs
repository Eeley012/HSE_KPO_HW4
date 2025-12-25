using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Orders.Service.Domain;

namespace Orders.Service.Infrastructure
{
    // класс-репозиторий, в нем есть контекст якобы базы данных,
    // можем искать заказы и что-то атомарно делать с имеющимися
    public class OrderRepository : IOrderRepository
    {
        private readonly DbImitationContext _dbContext;

        public OrderRepository(DbImitationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Order?> GetByIdAsync(Guid id)
        {
            var order = _dbContext.Orders.FirstOrDefault(o => o.Id == id);
            return Task.FromResult(order);
        }

        public Task SaveAsync(Order order, object? outboxEvent = null)
        {
            _dbContext.ExecuteTransaction(() =>
            {
                var existing = _dbContext.Orders.FirstOrDefault(o => o.Id == order.Id);
                if (existing == null)
                {
                    _dbContext.Orders.Add(order);
                }
                if (outboxEvent != null)
                {
                    var message = new OutboxMessage
                    {
                        Type = outboxEvent.GetType().Name,
                        Content = JsonSerializer.Serialize(outboxEvent)
                    };
                    _dbContext.OutboxMessages.Add(message);
                }
            });

            return Task.CompletedTask;
        }
    }
}