using Microsoft.AspNetCore.Mvc;
using Orders.Service.Domain;
using Orders.Service.UseCases.CreateOrder;

namespace Orders.Service.Controllers
{
    // высокоуровневая работа с http и json объектами
    [ApiController]
    [Route("api/orders")] 
    public class OrdersController : ControllerBase
    {
        private readonly ICreateOrderUseCase _createOrderUseCase;
        private readonly IOrderRepository _repository;
        
        public OrdersController(ICreateOrderUseCase createOrderUseCase, IOrderRepository repository)
        {
            _createOrderUseCase = createOrderUseCase;
            _repository = repository;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _createOrderUseCase.ExecuteAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var order = await _repository.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound(new { error = $"Order with ID {id} is not found" });
            }
            
            return Ok(new
            {
                order.Id,
                order.UserId,
                order.Amount,
                order.Description,
                order.Status,  
                StatusName = order.Status.ToString(),
                order.CreatedAt
            });
        }
        [HttpGet]
        public IActionResult GetAllOrders([FromServices] Infrastructure.DbImitationContext db)
        {
            var result = db.Orders.Select(o => new 
            {
                o.Id,
                o.UserId,
                o.Amount,
                o.Status,
                StatusName = o.Status.ToString()
            });
    
            return Ok(result);
        }
    }
}