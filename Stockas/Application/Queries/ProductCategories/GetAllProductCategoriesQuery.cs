using MediatR;
using Stockas.Models.DTOS;

namespace Stockas.Handlers.Queries
{
    public class GetAllProductCategoriesQuery : IRequest<List<ProductCategoryDto>>
    {
    }
}
