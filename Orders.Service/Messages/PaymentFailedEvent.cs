using System;

namespace Orders.Service.Messages
{
    // содержание сообщения с отменой оплаты (по какому счету и почему)
    public class PaymentFailedEvent
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}