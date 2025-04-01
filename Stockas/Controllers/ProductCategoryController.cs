using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stockas.Application.Commands;
using Stockas.Application.Validators;
using Stockas.Handlers.Queries;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Stockas.Controllers
{
    [Route("api/product-categories")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductCategoryController> _logger;

        public ProductCategoryController(IMediator mediator, ILogger<ProductCategoryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        // GET: api/<ProductCategoryController>
        [HttpGet]
        public async Task<IActionResult> GetAllProductCategories()
        {
            _logger.LogInformation("Received request to fetch all product categories");

            var categories = await _mediator.Send(new GetAllProductCategoriesQuery());

            if (categories == null || !categories.Any())
            {
                _logger.LogWarning("No product categories found");
                return NoContent(); 
            }

            return Ok(categories); 
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateProductCategoryCommand command)
        {
            var validator = new CreateProductCategoryValidator();
            var validationResult = validator.Validate(command);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateCategory), new { id = result.CategoryId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateProductCategoryCommand command)
        {
            command.CategoryId = id; 

            var updatedCategory = await _mediator.Send(command);
            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _mediator.Send(new DeleteProductCategoryCommand(id));
            return NoContent();
        }
    }
}
