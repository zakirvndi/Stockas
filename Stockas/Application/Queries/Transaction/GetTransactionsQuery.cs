using MediatR;
using Stockas.Models.DTOS;

namespace Stockas.Application.Queries.Transaction
{
    public class GetTransactionsQuery : IRequest<IEnumerable<TransactionDTO>>
    {
        public int UserId { get; set; }
        public string? GroupBy { get; set; }
        public string? SortBy { get; set; }
        public bool SortDesc { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
