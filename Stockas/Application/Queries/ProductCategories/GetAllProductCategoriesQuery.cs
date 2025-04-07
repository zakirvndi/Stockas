using MediatR;
using Stockas.Models.DTOS;

namespace Stockas.Application.Queries
{
    public class GetAllProductCategoriesQuery : IRequest<List<ProductCategoryDto>>
    {
        public int UserId { get; set; } 

    }
}
