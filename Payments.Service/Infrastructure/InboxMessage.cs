namespace Payments.Service.Infrastructure
{
    // сообщение в inbox c уникальным номером
    public class InboxMessage
    {
        public Guid Id { get; set; } 
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        
    }
}