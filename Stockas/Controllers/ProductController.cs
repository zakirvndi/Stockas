using MediatR;
using Microsoft.AspNetCore.Mvc;



namespace Stockas.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? orderBy = "name",
            [FromQuery] bool orderDesc = false,
            [FromQuery] bool groupByCategory = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetProductsQuery
            {
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
            command.UserId = 11; 

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateProduct), new { productId = result }, result);
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

            //userId sementara
            command.UserId = 11;

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            
            int userId = 1; 

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
