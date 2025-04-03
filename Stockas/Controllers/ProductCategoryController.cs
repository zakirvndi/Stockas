using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stockas.Application.Commands;
using Stockas.Application.Queries;
using Stockas.Application.Validators;
using System.Security.Claims;

namespace Stockas.Controllers
{
    [Route("api/product-categories")]
    [ApiController]
    [Authorize]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductCategoryController> _logger;

        public ProductCategoryController(IMediator mediator, ILogger<ProductCategoryController> logger)
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
        public async Task<IActionResult> GetAllProductCategories()
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized("Invalid User ID format in token.");

            _logger.LogInformation("Fetching product categories for user {UserId}", userId);

            var categories = await _mediator.Send(new GetAllProductCategoriesQuery(userId));

            if (categories == null || !categories.Any())
            {
                _logger.LogWarning("No product categories found for user {UserId}", userId);
                return NoContent();
            }

            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateProductCategoryCommand command)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized("Invalid User ID format in token.");

            command.UserId = userId;

            var validator = new CreateProductCategoryValidator();
            var validationResult = validator.Validate(command);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateCategory), new { id = result.CategoryId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateProductCategoryCommand command)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized("Invalid User ID format in token.");

            command.CategoryId = id;
            command.UserId = userId;

            var updatedCategory = await _mediator.Send(command);

            if (updatedCategory == null)
                return Forbid("You do not have permission to edit this category.");

            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized("Invalid User ID format in token.");

            var command = new DeleteProductCategoryCommand(id, userId);
            var result = await _mediator.Send(command);

            if (result == Unit.Value)
                return NoContent(); 

            return Forbid("You do not have permission to delete this category.");
        }

        //method untuk mengambil UserId dari token dan mengonversinya ke int.
        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out userId);
        }
    }
}
