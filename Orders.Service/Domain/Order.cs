using System;

namespace Orders.Service.Domain
{
    // заказ, можно пометить как оплаченный или отменить
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public decimal Amount { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Description { get; private set; }
        
        public Order(Guid userId, decimal amount, string description)
        {
            if (amount <= 0) throw new ArgumentException("Sum must be positive");

            Id = Guid.NewGuid();
            UserId = userId;
            Amount = amount;
            Description = description;
            Status = OrderStatus.New;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsPaid()
        {
            Status = OrderStatus.Paid;
        }

        public void MarkAsFailed()
        {
            Status = OrderStatus.Failed;
        }
    }
}