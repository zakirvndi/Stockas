using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stockas.Application.Commands;
using Stockas.Application.Queries;
using System.Security.Claims;

namespace Stockas.Controllers
{
    [Route("api/products")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IMediator mediator, ILogger<ProductController> logger)
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
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? orderBy = "name",
            [FromQuery] bool orderDesc = false,
            [FromQuery] bool groupByCategory = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetAuthenticatedUserId();

            _logger.LogInformation(
                "User {UserId} fetching products with params: OrderBy={OrderBy}, OrderDesc={OrderDesc}, GroupByCategory={GroupByCategory}, Page={Page}, PageSize={PageSize}",
                userId, orderBy, orderDesc, groupByCategory, page, pageSize);

            var query = new GetProductsQuery
            {
                UserId = userId,
                OrderBy = orderBy,
                OrderDesc = orderDesc,
                GroupByCategory = groupByCategory,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var userId = GetAuthenticatedUserId();
            command.UserId = userId;

            _logger.LogInformation(
                "User {UserId} creating a new product: Name={ProductName}, PurchasePrice={PurchasePrice}, SellingPrice={SellingPrice} , Category={CategoryId}",
                userId, command.ProductName, command.PurchasePrice,command.SellingPrice, command.CategoryId);

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateProduct), new { productId = result.ProductId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductCommand command)
        {
            if (command == null)
            {
                return BadRequest("Invalid request data.");
            }

            if (id != command.ProductId)
            {
                return BadRequest("Product ID in URL and body must match.");
            }

            var userId = GetAuthenticatedUserId();
            command.UserId = userId;

            _logger.LogInformation(
                "User {UserId} updating product {ProductId}: Name={ProductName}, PurchasePrice={PurchasePrice}, SellingPrice={SellingPrice}, Category={CategoryId}",
                userId, id, command.ProductName, command.PurchasePrice, command.SellingPrice, command.CategoryId);

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userId = GetAuthenticatedUserId();

            _logger.LogInformation(
                "User {UserId} deleting product {ProductId}", userId, id);

            var command = new DeleteProductCommand
            {
                ProductId = id,
                UserId = userId
            };

            await _mediator.Send(command);
            return NoContent();
        }
    }
}
