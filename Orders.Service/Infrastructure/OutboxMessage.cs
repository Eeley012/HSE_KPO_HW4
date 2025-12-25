namespace Orders.Service.Infrastructure
{
    // класс сообщения в outbox
    public class OutboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedDate { get; set; }
    }
}