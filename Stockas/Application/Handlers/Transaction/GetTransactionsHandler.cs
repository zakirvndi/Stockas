using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Stockas.Application.Queries.Transaction;
using Stockas.Entities;
using Stockas.Models.DTOS;
using System.Security.Claims;

namespace Stockas.Application.Handlers.Transaction
{
    public class GetTransactionsHandler : IRequestHandler<GetTransactionsQuery, IEnumerable<TransactionDTO>>
    {
        private readonly StockasContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetTransactionsHandler(
            StockasContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<TransactionDTO>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in claims");
            }

            var query = _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.Product)
                .Where(t => t.UserId == userId)
                .AsQueryable();

            // Apply sorting
            query = request.SortBy?.ToLower() switch
            {
                "date" => request.SortDesc
                    ? query.OrderByDescending(t => t.TransactionDate)
                    : query.OrderBy(t => t.TransactionDate),
                "amount" => request.SortDesc
                    ? query.OrderByDescending(t => t.Amount)
                    : query.OrderBy(t => t.Amount),
                _ => request.SortDesc
                    ? query.OrderByDescending(t => t.TransactionDate)
                    : query.OrderBy(t => t.TransactionDate),
            };

            // Apply grouping
            if (request.GroupBy?.ToLower() == "category")
            {
                query = query.OrderBy(t => t.Category.CategoryName);
            }
            else if (request.GroupBy?.ToLower() == "type")
            {
                query = query.OrderBy(t => t.Category.Type);
            }

            var transactions = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            if (!transactions.Any())
            {
                throw new KeyNotFoundException("No transactions found.");
            }

            return _mapper.Map<IEnumerable<TransactionDTO>>(transactions);
        }
    }

}
