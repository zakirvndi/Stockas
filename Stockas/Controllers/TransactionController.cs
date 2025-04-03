using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stockas.Application.Commands.Transaction;
using Stockas.Application.Queries.Transaction;
using System.Security.Claims;

namespace Stockas.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(
            IMediator mediator, 
            ILogger<TransactionsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        private int GetAuthenticatedUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("Unauthorized access attempt - User ID claim missing");
                throw new UnauthorizedAccessException("User ID not found in claims");
            }
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] string? groupBy = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDesc = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetAuthenticatedUserId();
            
            _logger.LogInformation(
                "User {UserId} fetching transactions with params: GroupBy={GroupBy}, SortBy={SortBy}, SortDesc={SortDesc}, Page={Page}, PageSize={PageSize}", 
                userId, groupBy, sortBy, sortDesc, page, pageSize);

            var transactions = await _mediator.Send(new GetTransactionsQuery
            {
                UserId = userId,
                GroupBy = groupBy,
                SortBy = sortBy,
                SortDesc = sortDesc,
                Page = page,
                PageSize = pageSize
            });

            return Ok(transactions);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand command)
        {
            var userId = GetAuthenticatedUserId();
            command.UserId = userId;

            _logger.LogInformation(
                "User {UserId} creating transaction: Amount={Amount}, Category={CategoryName}",
                userId, command.Amount, command.CategoryName);

            var result = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(CreateTransaction),
                new { id = result.TransactionId },
                result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(
            int id,
            [FromBody] UpdateTransactionCommand command)
        {
            var userId = GetAuthenticatedUserId();
            command.TransactionId = id;
            command.UserId = userId;

            _logger.LogInformation(
                "User {UserId} updating transaction {TransactionId} with: Amount={Amount}, Category={CategoryName}",
                userId, id, command.Amount, command.CategoryName);

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userId = GetAuthenticatedUserId();

            _logger.LogInformation(
                "User {UserId} deleting transaction {TransactionId}", 
                userId, id);

            await _mediator.Send(new DeleteTransactionCommand
            {
                TransactionId = id,
                UserId = userId
            });

            return NoContent();
        }
    }
}
