using System.ComponentModel.DataAnnotations;

namespace Orders.Service.UseCases.CreateOrder
{
    // дто заказа, данные пользователя, сумма и описание
    public class CreateOrderDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
    }
    
    
}