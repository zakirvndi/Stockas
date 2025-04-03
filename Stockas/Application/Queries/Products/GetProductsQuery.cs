using MediatR;
using Stockas.Models.DTOS;

namespace Stockas.Application.Queries
{
    public class GetProductsQuery : IRequest<IEnumerable<ProductDTO>>
    {
        public string? OrderBy { get; set; }
        public bool OrderDesc { get; set; } = false;
        public bool GroupByCategory { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int UserId { get; set; }
    }
}
