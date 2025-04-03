using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stockas.Application.Commands.TransactionCategory;
using Stockas.Application.Queries.TransactionCategory;
using System.Security.Claims;

namespace Stockas.Controllers
{
    [Route("api/transaction-categories")]
    [ApiController]
    [Authorize]
    public class TransactionCategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TransactionCategoriesController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionCategoriesController(
            IMediator mediator,
            ILogger<TransactionCategoriesController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetAuthenticatedUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Unauthorized access attempt - User ID claim missing");
                throw new UnauthorizedAccessException("User ID not found in claims");
            }
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionCategories()
        {
            var userId = GetAuthenticatedUserId();
            _logger.LogInformation("User {UserId} fetching transaction categories", userId);

            var categories = await _mediator.Send(new GetTransactionCategoriesQuery { UserId = userId });
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransactionCategory([FromBody] CreateTransactionCategoryCommand command)
        {
            var userId = GetAuthenticatedUserId();
            command.UserId = userId;

            _logger.LogInformation("User {UserId} creating transaction category: {CategoryName}",
                userId, command.CategoryName);

            var result = await _mediator.Send(command);
            return CreatedAtAction(
                nameof(CreateTransactionCategory),
                new { id = result.CategoryId },
                result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransactionCategory(int id, [FromBody] UpdateTransactionCategoryCommand command)
        {
            var userId = GetAuthenticatedUserId();
            command.CategoryId = id;
            command.UserId = userId;

            _logger.LogInformation("User {UserId} updating transaction category {CategoryId}",
                userId, id);

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransactionCategory(int id)
        {
            var userId = GetAuthenticatedUserId();

            _logger.LogInformation("User {UserId} deleting transaction category {CategoryId}",
                userId, id);

            await _mediator.Send(new DeleteTransactionCategoryCommand
            {
                CategoryId = id,
                UserId = userId
            });

            return NoContent();
        }
    }
}
