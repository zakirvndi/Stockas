using MediatR;
using Stockas.Models.DTOS;

namespace Stockas.Application.Commands
{
    public class CreateProductCategoryCommand : IRequest<ProductCategoryDto>
    {
        public string CategoryName { get; set; }

        public CreateProductCategoryCommand(string categoryName)
        {
            CategoryName = categoryName;
        }
    }
}
