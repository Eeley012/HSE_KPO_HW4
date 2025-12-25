using System.ComponentModel.DataAnnotations;

namespace Payments.Service.UseCases.CreateAccount
{ 
    // дто создания счета
    public class CreateAccountDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Range(0, 1000000)]
        public decimal InitialBalance { get; set; }
    }
}