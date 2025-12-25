namespace Orders.Service.UseCases.CreateOrder;

// интерфейс создания заказа
public interface ICreateOrderUseCase
{
    Task<CreateOrderResult> ExecuteAsync(CreateOrderDto dto);
}