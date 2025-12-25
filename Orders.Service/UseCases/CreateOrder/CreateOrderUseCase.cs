using Orders.Service.Domain;
using Orders.Service.Messages;

namespace Orders.Service.UseCases.CreateOrder
{
    // класс для создания заказа
    public class CreateOrderUseCase : ICreateOrderUseCase
    {
        private readonly IOrderRepository _repository;

        public CreateOrderUseCase(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateOrderResult> ExecuteAsync(CreateOrderDto dto)
        {
            var order = new Order(dto.UserId, dto.Amount, dto.Description);
            
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                UserId = order.UserId,
                Amount = order.Amount
            };

            // одновременно сохраняем в имитацию БД и сообщение о действии и сам заказ
            await _repository.SaveAsync(order, orderCreatedEvent);
            
            return new CreateOrderResult { OrderId = order.Id };
        }
    }
}