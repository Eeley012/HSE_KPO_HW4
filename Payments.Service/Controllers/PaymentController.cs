using Microsoft.AspNetCore.Mvc;
using Payments.Service.Domain;
using Payments.Service.UseCases.CreateAccount;
using Payments.Service.UseCases.Deposit;

namespace Payments.Service.Controllers
{
    // высокоуровневая работа с http и json объектами
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly ICreateAccountUseCase _createAccountUseCase;
        private readonly IDepositUseCase _depositUseCase;
        private readonly IAccountRepository _repository;

        public PaymentsController(ICreateAccountUseCase createAccountUseCase, IDepositUseCase depositUseCase, IAccountRepository repository)
        {
            _createAccountUseCase = createAccountUseCase;
            _depositUseCase = depositUseCase;
            _repository = repository;
        }
        
        [HttpPost("accounts")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _createAccountUseCase.ExecuteAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] DepositDto dto)
        {
            try 
            {
                await _depositUseCase.ExecuteAsync(dto);
                return Ok(new { message = "Balance updated" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("accounts/{userId}")]
        public async Task<IActionResult> GetAccount(Guid userId)
        {
            var account = await _repository.GetByUserIdAsync(userId);
            if (account == null) return NotFound();
            return Ok(account);
        }
    }
}