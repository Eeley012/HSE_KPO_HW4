namespace Payments.Service.Messages
{
    // сообщение об успешной оплате (просто id счета)
    public class PaymentSucceededEvent
    {
        public Guid OrderId { get; set; }
    }
}