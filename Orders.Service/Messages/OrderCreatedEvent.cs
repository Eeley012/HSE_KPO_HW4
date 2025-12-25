using System;

namespace Orders.Service.Messages
{
    // содержание сообщения о создании заказа
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
    }
}