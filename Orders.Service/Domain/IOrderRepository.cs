// интерфейс взаимодействуия с имитацией базы данных
namespace Orders.Service.Domain
{
    public interface IOrderRepository
    {
        Task SaveAsync(Order order, object? outboxEvent = null);
        Task<Order?> GetByIdAsync(Guid id);
    }
}